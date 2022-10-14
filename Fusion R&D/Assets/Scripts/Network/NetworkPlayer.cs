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



    #region Networked
    [Networked(OnChanged = nameof(OnNicknameChanged))]
    public NetworkString<_16> Nickname { get; set; }

    // ����Ʈ Ŭ���̾�Ʈ ��ū hash
    [Networked]
    public int Token { get; set; }
    #endregion



    bool _isPublicJoinMessageSent = false;

    public LocalCameraHandler LocalCameraHandler;
    public GameObject LocalUI;



    #region Other Components

    NetworkInGameMessages _networkInGameMessages;

    #endregion




    private void Awake()
    {
        _networkInGameMessages = GetComponent<NetworkInGameMessages>();
    }

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

            // null�� �ƴ϶�� mainī�޶� �ʿ����
            if (Camera.main != null)
            {
                Camera.main.gameObject.SetActive(false);
            }

            // ������ 1���� ����� �����ʸ� ���
            AudioListener audioListener = GetComponentInChildren<AudioListener>(true);
            audioListener.enabled = true;

            // ����ī�޶� �ѱ�
            LocalCameraHandler.LocalCam.enabled = true;

            // �θ�κ��� ����ī�޶� �и�
            LocalCameraHandler.transform.parent = null;

            LocalUI.SetActive(true);

            RPC_SettingNickname(PlayerPrefs.GetString("PlayerNickname"));

        }
        else
        {
            Debug.Log("����Ʈ �÷��̾� ����");

            //// �����÷��̾ �ƴҽ� ī�޶� disable
            //Camera localCamera = GetComponentInChildren<Camera>();
            //localCamera.enabled = false;
            LocalCameraHandler.LocalCam.enabled = false;

            // ����Ʈ�÷��̾��� ä��â�� ������ �������ʵ��� ��������
            LocalUI.SetActive(false);

            // ������ �ϳ��� ����������ʸ� �ʿ��ϹǷ� ����Ʈ�÷��̾��� ����� �����ʸ� disable����
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

        }

        // fusion���� �����ϴ� ����� �ϳ�
        // Set the player as a player object
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        // ���̾��Ű���� �̸����� ����
        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            // �Ѹ��� ������ ��� �÷��̾� Left �޽��� ����
            //_networkInGameMessages.SendInGameMessage(Nickname.Value, "Left");

            // playerRef�� ���޹ް�, networkobject�� ������
            // �̹� ���̽��� ��� ���� networkObject�� ����
            if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
            {
                // ���⼭ ��� �÷��̾ �ƴ� ���� �÷��̾ �����ϰԲ� ó������
                if (playerLeftNetworkObject == Object)
                {
                    // ������ �ڵ���� ȣ��Ʈ���׸� left�޽����� ��µ�
                    //_networkInGameMessages.SendInGameMessage(Nickname.ToString(), "Left");

                    // ����� �۵������� ��������� �ľ��ؾ��ҰͰ���
                    // ������ ���� ������ ������ rpc�� ���� �޽����� ��������� �ð��� ���ġ����
                    Local.GetComponent<NetworkInGameMessages>().SendInGameRPCMessage(playerLeftNetworkObject.GetComponent<NetworkPlayer>().Nickname.ToString(), "Left");
                }
            }
        }

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


        if (!_isPublicJoinMessageSent)
        {
            _networkInGameMessages.SendInGameRPCMessage(nickname, "joined");

            _isPublicJoinMessageSent = true;
        }
    }

    private void OnDestroy()
    {
        // ���ο� ��Ʈ��ũ �÷��̾ �����ɶ� ���� ����� ī�޶� ������
        if (LocalCameraHandler != null)
        {
            Destroy(LocalCameraHandler.gameObject);
        }
    }
}
