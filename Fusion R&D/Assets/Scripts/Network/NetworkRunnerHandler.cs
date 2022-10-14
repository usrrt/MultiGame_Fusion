using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using System.Linq;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner NetworkRunnerPrefab;

    NetworkRunner _networkRunner;

    private void Start()
    {
        // public���� ���� ��Ʈ��ũ���ʸ� private��Ʈ��ũ ���ʿ� clone
        _networkRunner = Instantiate(NetworkRunnerPrefab);
        _networkRunner.name = "Network runner";

        // TaskŸ������ NetworkRunner�� �������� ������
        //var clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null, GameManager.Instance.GetConnectionToken());
        var clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, GameManager.Instance.GetConnectionToken(), NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

        Debug.Log($"���� ��Ʈ��ũ���� ����");
    }

    // ���ο� ȣ��Ʈ�� ������ ȣ��Ʈ�� �����ߴ������� �״�� �����ؾ���
    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        _networkRunner = Instantiate(NetworkRunnerPrefab);
        _networkRunner.name = "Network runner - Migrated";

        var clientTask = InitializeNetworkRunnerHostMigration(_networkRunner, hostMigrationToken);

        Debug.Log("ȣ��Ʈ���� �Ѱ���");
    }


    // scenemanager�� ����
    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            // �̹� ���� �ִ� networked ������Ʈ�� ó����
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneManager;
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode mode, byte[] connectionToken, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            Address = address,
            Scene = scene,
            SessionName = "TestRoom",
            Initialized = initialized, 
            SceneManager = sceneManager,

            // ������ ���ڸ� �Ѱ��༭ ����
            // ��ū�� Ŭ���̾�Ʈ�� ���ؼ� �����ȴ�
            ConnectionToken = connectionToken,
        });

    }

  

    // ���ο� ȣ��Ʈ���� �Ѱ��� ������
    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            // hostMigrationToken�� ������ ������ֱ⶧���� �ٽ� �Ѱ��� �ʿ䰡����.
            //GameMode = mode, 
            //Address = address,
            //Scene = scene,
            //SessionName = "TestRoom",
            //Initialized = initialized,

            SceneManager = sceneManager,
            HostMigrationToken = hostMigrationToken,    // ���ο� runner���� �ʿ��� ������ ���������
            HostMigrationResume = HostMigrationResume,  // simulation�� �簳�Ҷ� ȣ��

            // ���ο� ȣ��Ʈ���� ���ӸŴ����� ����� ��ū ������ �Ѱ��ش�
            ConnectionToken = GameManager.Instance.GetConnectionToken(),

        });

    }

    // ���ο� runner�� �����Ҷ� ����Ǵ� �κ�
    private void HostMigrationResume(NetworkRunner runner)
    { 
        Debug.Log("Host Migration Resume ����");

        // old Host���� �� network object������ ���� ������
        foreach (var resumeNetworkObject in runner.GetResumeSnapshotNetworkObjects())
        {
            // player object�� NetworkCharacterControllerPrototypeCustom�� ������������ ������ �����Ѵ�
            if (resumeNetworkObject.TryGetBehaviour<NetworkCharacterControllerPrototypeCustom>(out var characterController))
            {
                // ���� ���� -> ���ο� ȣ��Ʈ�� ��������� �����Ұ�
                runner.Spawn(resumeNetworkObject, position: characterController.ReadPosition(), rotation: characterController.ReadRotation(), onBeforeSpawned: (runner, newNetworkObject)
                    =>
                {
                    // old object���� ������������ new object�� ����
                    newNetworkObject.CopyStateFrom(resumeNetworkObject);

                    // ������ū�� ���ο� network player �������ֱ�
                    if (resumeNetworkObject.TryGetBehaviour<NetworkPlayer>(out var oldNetworkPlayer))
                    {
                        // �翬���� ���� ���� �÷��̾���ū ����
                        FindObjectOfType<Spawner>().SetConnetionTokenMapping(oldNetworkPlayer.Token, newNetworkObject.GetComponent<NetworkPlayer>());
                    }

                });
            }
        }




        Debug.Log("Host Migration Resume ����");
    }

    
}
