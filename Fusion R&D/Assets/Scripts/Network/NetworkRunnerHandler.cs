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

        var clientTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

        Debug.Log($"서버 네트워크러너 시작");
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode mode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            // 이미 씬에 있는 networked 오브젝트를 처리함
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            Address = address,
            Scene = scene,
            SessionName = "TestRoom",
            Initialized = initialized,
            SceneManager = sceneManager
        });

    }
}
