using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    // 스폰할 플레이어 프리팹
    public NetworkPlayer PlayerPrefab;

    // 딕셔너리를 사용해 토큰 ID와 새로 생성된 플레이어 Mapping하기
    Dictionary<int, NetworkPlayer> _mapTokenIDWithNetworkPlayer;

    CharacterInputHandler _characterInputHandler;


    private void Awake()
    {
        // 새로운 dictionary 생성
       _mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();
    }

    
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("서버와 연결됨");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("서버와 연결 실패");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("연결 요청받음");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("서버로부터 연결끊김");
    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host Migration is working");

        // await는 async메소드에서만 쓸수있다
        // shut down the current runner
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        // old runner로부터 데이터 가져오기
        // network runner handler를 찾고 host migration 시작하기(starthostmigration메소드 정의)
        FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // CharacterInputHandler가 없는 로컬플레이어에게
        if (_characterInputHandler == null && NetworkPlayer.Local != null)
        {
            // CharacterInputHandler컴포넌트 달아줌
            _characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();
        }

        // CharacterInputHandler가지고있으면
        if (_characterInputHandler != null)
        {
            // 입력을 set해줌
            input.Set(_characterInputHandler.GetNetworkInput());
        }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    int GetPlayerToken(NetworkRunner runner, PlayerRef player)
    {
        // 로컬플레이어의 경우 GetConnectionToken
        if (runner.LocalPlayer == this)
        {
             return ConnectionTokenUtils.HashToken(GameManager.Instance.GetConnectionToken());
        }
        else
        {
            // 리모트플레이어의 경우
            // 클라이언트가 호스트로 연결될때 저장된 연결토큰 가져오기
            var token = runner.GetPlayerConnectionToken(player);

            if (token != null)
            {
                return ConnectionTokenUtils.HashToken(token);
            }

            Debug.LogError("GetPlayerToken 유호한 token을 return함");
            return 0; // invaild
        }
        
    }

    public void SetConnetionTokenMapping(int token, NetworkPlayer networkPlayer)
    {
        _mapTokenIDWithNetworkPlayer.Add(token, networkPlayer);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // 서버가 다시 시작할때 기존의 플레이어를 조정해야한다 새로운 플레이어를 스폰할게 아니라
            // 이것을 구분짓기위해서 fusion에선 connectionToken이 존재함
            // player를 위한 토큰얻기
            int playerToken = GetPlayerToken(runner, player);

            Debug.Log($"우리가 서버야. 플레이어 스폰중. Connection Token : {playerToken}");


            // 항상 새로운 플레이어를 spawn할 필요없음
            // 이미 존재하거나 아닌경우인 조건을 통해 spawn할지말지 결정가능
            // dictoinary를 통해 token값을 사용해 network플레이어 구별하여 스폰함

            // 서버에 이미 기록된 토큰인지 체크
            if (_mapTokenIDWithNetworkPlayer.TryGetValue(playerToken, out NetworkPlayer networkPlayer))
            {
                Debug.Log($"토큰 {playerToken}에 대한 예전 연결토큰 찾음. 컨트롤할 플레이어를 할당하시오");

                networkPlayer.GetComponent<NetworkObject>().AssignInputAuthority(player);

                networkPlayer.Spawned();
            }
            else
            {
                Debug.Log($"새로운 플레이어를 위한 연결 토큰 {playerToken}생성");
                // 플레이어를 찾지 못했다면 새로운 플레이어 스폰함
                // 이 정보를 dictionary에 저장해야함 => 변수에 담음
                NetworkPlayer spawnedNetworkPlayer = runner.Spawn(PlayerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);

                // 생성된 토큰 플레이어토큰에 저장
                spawnedNetworkPlayer.Token = playerToken;

                // 토큰과 플레이어 mapping해주기
                _mapTokenIDWithNetworkPlayer[playerToken] = spawnedNetworkPlayer;

            }
        }
        else
        {
            Debug.Log("플레이어 join");
        }
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("셧다운");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    
}
