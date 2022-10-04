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
            Debug.Log("���� �÷��̾� ����");

            Local = this;

            // �����÷��̾� �� ���̾� ����
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            // mainī�޶� �ʿ����
            Camera.main.gameObject.SetActive(false);

        }
        else
        {
            Debug.Log("����Ʈ �÷��̾� ����");

            // �����÷��̾ �ƴҽ� ī�޶� disable
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            // ������ �ϳ��� ����������ʸ� �ʿ��ϹǷ� ����Ʈ�÷��̾��� ����� �����ʸ� disable����
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

        }

        // ���̾��Ű���� �̸����� ����
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
