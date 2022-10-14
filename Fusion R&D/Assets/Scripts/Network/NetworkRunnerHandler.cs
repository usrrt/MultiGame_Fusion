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
        // public으로 받은 네트워크러너를 private네트워크 러너에 clone
        _networkRunner = Instantiate(NetworkRunnerPrefab);
        _networkRunner.name = "Network runner";

        // Task타입으로 NetworkRunner의 정보들을 저장함
        //var clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null, GameManager.Instance.GetConnectionToken());
        var clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, GameManager.Instance.GetConnectionToken(), NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

        Debug.Log($"서버 네트워크러너 시작");
    }

    // 새로운 호스트도 기존의 호스트가 진행했던순서를 그대로 실행해야함
    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        _networkRunner = Instantiate(NetworkRunnerPrefab);
        _networkRunner.name = "Network runner - Migrated";

        var clientTask = InitializeNetworkRunnerHostMigration(_networkRunner, hostMigrationToken);

        Debug.Log("호스트권한 넘겨줌");
    }


    // scenemanager를 리턴
    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            // 이미 씬에 있는 networked 오브젝트를 처리함
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

            // 고유의 숫자를 넘겨줘서 구분
            // 토큰은 클라이언트에 의해서 생성된다
            ConnectionToken = connectionToken,
        });

    }

  

    // 새로운 호스트에게 넘겨줄 정보들
    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            // hostMigrationToken에 정보가 담겨져있기때문에 다시 넘겨줄 필요가없다.
            //GameMode = mode, 
            //Address = address,
            //Scene = scene,
            //SessionName = "TestRoom",
            //Initialized = initialized,

            SceneManager = sceneManager,
            HostMigrationToken = hostMigrationToken,    // 새로운 runner에게 필요한 정보가 담겨져있음
            HostMigrationResume = HostMigrationResume,  // simulation을 재개할때 호출

            // 새로운 호스트에게 게임매니저에 저장된 토큰 정보를 넘겨준다
            ConnectionToken = GameManager.Instance.GetConnectionToken(),

        });

    }

    // 새로운 runner를 시작할때 실행되는 부분
    private void HostMigrationResume(NetworkRunner runner)
    { 
        Debug.Log("Host Migration Resume 시작");

        // old Host에서 각 network object에대한 참조 가져옴
        foreach (var resumeNetworkObject in runner.GetResumeSnapshotNetworkObjects())
        {
            // player object가 NetworkCharacterControllerPrototypeCustom을 가지고있으면 구문을 실행한다
            if (resumeNetworkObject.TryGetBehaviour<NetworkCharacterControllerPrototypeCustom>(out var characterController))
            {
                // 정보 복사 -> 새로운 호스트를 만들기전에 복사할것
                runner.Spawn(resumeNetworkObject, position: characterController.ReadPosition(), rotation: characterController.ReadRotation(), onBeforeSpawned: (runner, newNetworkObject)
                    =>
                {
                    // old object에서 복사한정보를 new object에 전달
                    newNetworkObject.CopyStateFrom(resumeNetworkObject);

                    // 연결토큰과 새로운 network player 연결해주기
                    if (resumeNetworkObject.TryGetBehaviour<NetworkPlayer>(out var oldNetworkPlayer))
                    {
                        // 재연결을 위한 예전 플레이어토큰 저장
                        FindObjectOfType<Spawner>().SetConnetionTokenMapping(oldNetworkPlayer.Token, newNetworkObject.GetComponent<NetworkPlayer>());
                    }

                });
            }
        }




        Debug.Log("Host Migration Resume 종료");
    }

    
}
