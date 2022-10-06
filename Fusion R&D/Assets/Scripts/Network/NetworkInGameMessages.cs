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

    // rpc를 통해 서버로부터 받은 메시지들을 모두에게 뿌림(?)
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_InGameMessage(string message, RpcInfo info = default)
    {
        Debug.Log($"[RPC] in game message {message}");

        if (InGameMessageUIHandler == null)
        {
            // 로컬플레이어로 생성될때 InGameMessageUIHandler컴포넌트를 가져온다
            InGameMessageUIHandler = NetworkPlayer.Local.LocalCameraHandler.GetComponentInChildren<InGameMessageUIHandler>();
        }

        if (InGameMessageUIHandler != null)
        {
            InGameMessageUIHandler.OnGameMessageReceived(message);
        }
    }
}
