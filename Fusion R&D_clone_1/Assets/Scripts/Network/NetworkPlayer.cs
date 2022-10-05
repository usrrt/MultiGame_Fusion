using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using System;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{

    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    public static NetworkPlayer Local { get; set; }

    public TextMeshProUGUI PlayerNicknameText;
    public Transform playerModel;

    [Networked(OnChanged = nameof(OnNicknameChanged))]
    public NetworkString<_16> Nickname { get; set; }

    
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

            RPC_SettingNickname(PlayerPrefs.GetString("PlayerNickname"));

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
        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    private void OnNicknameChanged()
    {
        Debug.Log($"{gameObject.name}�� �г��� : {Nickname}");

        PlayerNicknameText.text = Nickname.ToString();
    }

    // Networked OnChanged �ݹ� �޼���
    static void OnNicknameChanged(Changed<NetworkPlayer> changed)
    {
        Debug.Log($"�ٲ� ��{changed.Behaviour.Nickname}");

        changed.Behaviour.OnNicknameChanged();
    }

    // RPC�� ���� ���� ������ �г����� ������ �˷�����Ѵ�
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SettingNickname(string nickname, RpcInfo info = default)
    {
        Debug.Log("[RPC] �г��� ����");
        this.Nickname = nickname;
    }
}
