using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class NetworkInGameMessages : NetworkBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    InGameMessageUIHandler InGameMessageUIHandler;
    
    void Start()
    {
        
    }

    public void SendInGameRPCMessage(string userNickname, string message)
    {
        RPC_InGameMessage($"{userNickname} : {message}");
    }

    // rpc�� ���� �����κ��� ���� �޽������� ��ο��� �Ѹ�(?)
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_InGameMessage(string message, RpcInfo info = default)
    {
        Debug.Log($"[RPC] in game message {message}");

        if (InGameMessageUIHandler == null)
        {
            // �����÷��̾�� �����ɶ� InGameMessageUIHandler������Ʈ�� �����´�
            InGameMessageUIHandler = NetworkPlayer.Local.LocalCameraHandler.GetComponentInChildren<InGameMessageUIHandler>();
        }

        if (InGameMessageUIHandler != null)
        {
            InGameMessageUIHandler.OnGameMessageReceived(message);
        }
    }
}
