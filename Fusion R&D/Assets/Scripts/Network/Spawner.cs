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

    // ������ �÷��̾� ������
    public NetworkPlayer PlayerPrefab;

    // ��ųʸ��� ����� ��ū ID�� ���� ������ �÷��̾� Mapping�ϱ�
    Dictionary<int, NetworkPlayer> _mapTokenIDWithNetworkPlayer;

    CharacterInputHandler _characterInputHandler;


    private void Awake()
    {
        // ���ο� dictionary ����
       _mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();
    }

    
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("������ �����");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("������ ���� ����");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("���� ��û����");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("�����κ��� �������");
    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host Migration is working");

        // await�� async�޼ҵ忡���� �����ִ�
        // shut down the current runner
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        // old runner�κ��� ������ ��������
        // network runner handler�� ã�� host migration �����ϱ�(starthostmigration�޼ҵ� ����)
        FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // CharacterInputHandler�� ���� �����÷��̾��
        if (_characterInputHandler == null && NetworkPlayer.Local != null)
        {
            // CharacterInputHandler������Ʈ �޾���
            _characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();
        }

        // CharacterInputHandler������������
        if (_characterInputHandler != null)
        {
            // �Է��� set����
            input.Set(_characterInputHandler.GetNetworkInput());
        }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    int GetPlayerToken(NetworkRunner runner, PlayerRef player)
    {
        // �����÷��̾��� ��� GetConnectionToken
        if (runner.LocalPlayer == this)
        {
             return ConnectionTokenUtils.HashToken(GameManager.Instance.GetConnectionToken());
        }
        else
        {
            // ����Ʈ�÷��̾��� ���
            // Ŭ���̾�Ʈ�� ȣ��Ʈ�� ����ɶ� ����� ������ū ��������
            var token = runner.GetPlayerConnectionToken(player);

            if (token != null)
            {
                return ConnectionTokenUtils.HashToken(token);
            }

            Debug.LogError("GetPlayerToken ��ȣ�� token�� return��");
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
            // ������ �ٽ� �����Ҷ� ������ �÷��̾ �����ؾ��Ѵ� ���ο� �÷��̾ �����Ұ� �ƴ϶�
            // �̰��� �����������ؼ� fusion���� connectionToken�� ������
            // player�� ���� ��ū���
            int playerToken = GetPlayerToken(runner, player);

            Debug.Log($"�츮�� ������. �÷��̾� ������. Connection Token : {playerToken}");


            // �׻� ���ο� �÷��̾ spawn�� �ʿ����
            // �̹� �����ϰų� �ƴѰ���� ������ ���� spawn�������� ��������
            // dictoinary�� ���� token���� ����� network�÷��̾� �����Ͽ� ������

            // ������ �̹� ��ϵ� ��ū���� üũ
            if (_mapTokenIDWithNetworkPlayer.TryGetValue(playerToken, out NetworkPlayer networkPlayer))
            {
                Debug.Log($"��ū {playerToken}�� ���� ���� ������ū ã��. ��Ʈ���� �÷��̾ �Ҵ��Ͻÿ�");

                networkPlayer.GetComponent<NetworkObject>().AssignInputAuthority(player);

                networkPlayer.Spawned();
            }
            else
            {
                Debug.Log($"���ο� �÷��̾ ���� ���� ��ū {playerToken}����");
                // �÷��̾ ã�� ���ߴٸ� ���ο� �÷��̾� ������
                // �� ������ dictionary�� �����ؾ��� => ������ ����
                NetworkPlayer spawnedNetworkPlayer = runner.Spawn(PlayerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);

                // ������ ��ū �÷��̾���ū�� ����
                spawnedNetworkPlayer.Token = playerToken;

                // ��ū�� �÷��̾� mapping���ֱ�
                _mapTokenIDWithNetworkPlayer[playerToken] = spawnedNetworkPlayer;

            }
        }
        else
        {
            Debug.Log("�÷��̾� join");
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
        Debug.Log("�˴ٿ�");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    
}
