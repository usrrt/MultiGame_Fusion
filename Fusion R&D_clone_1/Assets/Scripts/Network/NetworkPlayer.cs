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
            Debug.Log("로컬 플레이어 생성");

            Local = this;

            // 로컬플레이어 모델 레이어 세팅
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            // main카메라 필요없음
            Camera.main.gameObject.SetActive(false);

            RPC_SettingNickname(PlayerPrefs.GetString("PlayerNickname"));

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
        Debug.Log($"{gameObject.name}의 닉네임 : {Nickname}");

        PlayerNicknameText.text = Nickname.ToString();
    }

    // Networked OnChanged 콜백 메서드
    static void OnNicknameChanged(Changed<NetworkPlayer> changed)
    {
        Debug.Log($"바뀐 값{changed.Behaviour.Nickname}");

        changed.Behaviour.OnNicknameChanged();
    }

    // RPC를 통해 내가 설정한 닉네임을 서버에 알려줘야한다
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SettingNickname(string nickname, RpcInfo info = default)
    {
        Debug.Log("[RPC] 닉네임 세팅");
        this.Nickname = nickname;
    }
}
