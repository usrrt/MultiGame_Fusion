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

    // 리모트 클라이언트 토큰 hash
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
            Debug.Log("로컬 플레이어 생성");

            Local = this;

            // 로컬플레이어 모델 레이어 세팅
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            // null이 아니라면 main카메라 필요없음
            if (Camera.main != null)
            {
                Camera.main.gameObject.SetActive(false);
            }

            // 씬에서 1개의 오디오 리스너만 허용
            AudioListener audioListener = GetComponentInChildren<AudioListener>(true);
            audioListener.enabled = true;

            // 로컬카메라 켜기
            LocalCameraHandler.LocalCam.enabled = true;

            // 부모로부터 로컬카메라 분리
            LocalCameraHandler.transform.parent = null;

            LocalUI.SetActive(true);

            RPC_SettingNickname(PlayerPrefs.GetString("PlayerNickname"));

        }
        else
        {
            Debug.Log("리모트 플레이어 생성");

            //// 로컬플레이어가 아닐시 카메라 disable
            //Camera localCamera = GetComponentInChildren<Camera>();
            //localCamera.enabled = false;
            LocalCameraHandler.LocalCam.enabled = false;

            // 리모트플레이어의 채팅창이 나에게 보이지않도록 꺼버리기
            LocalUI.SetActive(false);

            // 씬에선 하나의 오디오리스너만 필요하므로 리모트플레이어의 오디오 리스너를 disable해줌
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

        }

        // fusion에서 지원하는 기능중 하나
        // Set the player as a player object
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        // 하이어라키에서 이름구별 가능
        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            // 한명이 나가면 모든 플레이어 Left 메시지 보냄
            //_networkInGameMessages.SendInGameMessage(Nickname.Value, "Left");

            // playerRef를 공급받고, networkobject를 리턴함
            // 이번 케이스의 경우 떠난 networkObject를 리턴
            if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
            {
                // 여기서 모든 플레이어가 아닌 떠난 플레이어만 구분하게끔 처리해줌
                if (playerLeftNetworkObject == Object)
                {
                    // 기존의 코드사용시 호스트한테만 left메시지가 출력됨
                    //_networkInGameMessages.SendInGameMessage(Nickname.ToString(), "Left");

                    // 제대로 작동하지만 어떤원리인지 파악해야할것같음
                    // 연결을 끊고 게임을 떠나면 rpc에 떠남 메시지를 보내기까지 시간이 충분치않음
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


        if (!_isPublicJoinMessageSent)
        {
            _networkInGameMessages.SendInGameRPCMessage(nickname, "joined");

            _isPublicJoinMessageSent = true;
        }
    }

    private void OnDestroy()
    {
        // 새로운 네트워크 플레이어가 생성될때 같이 생기는 카메라를 삭제함
        if (LocalCameraHandler != null)
        {
            Destroy(LocalCameraHandler.gameObject);
        }
    }
}
