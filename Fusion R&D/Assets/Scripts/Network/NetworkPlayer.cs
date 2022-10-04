using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{

    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    public static NetworkPlayer Local { get; set; }

    public Transform playerModel;

    void Start()
    {
        
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Debug.Log("로컬 플레이어 생성");

            Local = this;

            // 로컬플레이어 모델 레이어 세팅
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            // main카메라 필요없음
            Camera.main.gameObject.SetActive(false);

        }
        else
        {
            Debug.Log("리모트 플레이어 생성");

            // 로컬플레이어가 아닐시 카메라 disable
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            // 씬에선 하나의 오디오리스너만 필요하므로 리모트플레이어의 오디오 리스너를 disable해줌
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

        }

        // 하이어라키에서 이름구별 가능
        //transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
