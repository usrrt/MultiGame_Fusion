using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    [Networked(OnChanged = nameof(OnHPChanged))]
    byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool IsDead { get; set; }

    bool _isInitialized = false;

    const byte _startingHP = 5;

    public Color UIOnHitColor;
    public Image UIOnHitImg;

    public MeshRenderer BodyMeshRenderer;
    Color _defaultMeshBodyColor;

    public GameObject PlayerModel;
    // 튜토리얼에선 오브젝트를 instantiate를 사용해 생성하고있지만
    // 오브젝트풀링을 사용해 파티클 가져오는게 퍼포먼스 향상에 도움을준다
    public GameObject DeathPlayerPrefab; 

    #region Other Components

    // 플레이어가 총에 두번씩 맞는거 방지
    HitboxRoot _hitboxRoot;
    CharacterMovementHandler _characterMovementHandler;
    NetworkInGameMessages _networkInGameMessages;
    NetworkPlayer _networkPlayer;

    #endregion

    private void Awake()
    {
        _hitboxRoot = GetComponentInChildren<HitboxRoot>();
        _characterMovementHandler = GetComponent<CharacterMovementHandler>();
        _networkInGameMessages = GetComponent<NetworkInGameMessages>();
        _networkPlayer = GetComponent<NetworkPlayer>();
    }

    void Start()
    {
        HP = _startingHP;
        IsDead = false;

        _defaultMeshBodyColor = BodyMeshRenderer.material.color;

        _isInitialized = true;
    }

    IEnumerator OnHit()
    {
        BodyMeshRenderer.material.color = Color.white;

        if (Object.HasInputAuthority)
        {
            UIOnHitImg.color = UIOnHitColor;
        }

        yield return new WaitForSeconds(0.2f);

        BodyMeshRenderer.material.color = _defaultMeshBodyColor;

        if (!IsDead && Object.HasInputAuthority)
        {
            UIOnHitImg.color = new Color(0, 0, 0, 0);
        }
    }

    IEnumerator ServerRevive()
    {
        yield return new WaitForSeconds(2f);

        _characterMovementHandler.RequestRespawn();
    }

    private static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log("is dead " + changed.Behaviour.IsDead);

        bool isDeathCurrent = changed.Behaviour.IsDead;

        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.IsDead;

        // 최근에 죽음상태라면
        if (isDeathCurrent)
        {
            // 죽음 메소드 실행
            changed.Behaviour.OnDeath();
        }
        // 최근에 살아있고 과거에 죽은상태였다면
        else if (!isDeathCurrent && isDeadOld)
        {
            // 소생 메소드 실행
            changed.Behaviour.OnRevive();
        }
    }

    private static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log("HP value " + changed.Behaviour.HP);

        byte newHP = changed.Behaviour.HP;

        changed.LoadOld();
       

        byte oldHP = changed.Behaviour.HP;

        // HP변화 체크 -> 최신hp가 이전의hp보다 작다 -> hp가 감소된상황
        if (newHP < oldHP)
        {
            changed.Behaviour.OnHPReduced();
        }
    }

    void OnHPReduced()
    {
        if (!_isInitialized)
        {
            return;
        }

        StartCoroutine(OnHit());
    }

    public void OnRespawn()
    {
        // reset
        HP = _startingHP;
        IsDead = false;
    }

    // 오직 서버에 의해서만 실행됨
    public void OnTakeDamage(string damageCauseByPlayerNickname)
    {
        // 살아있을때만 데미지 입음
        if (IsDead)
        {
            return;
        }

        HP -= 1;

        Debug.Log($"{transform.name} 남은 체력 : {HP}");

        if (HP <= 0)
        {
            // damageCauseByPlayerNickname => 죽인사람 => 로컬 플레이어
            // _networkPlayer.Nickname.ToString() => 죽은사람 => 리모트 플레이어 
            _networkInGameMessages.SendInGameRPCMessage(damageCauseByPlayerNickname, $"killed {_networkPlayer.Nickname.ToString()}");

            StartCoroutine(ServerRevive());
            IsDead = true;
        }
    }

    private void OnDeath()
    {
        Debug.Log("OnDeath");

        PlayerModel.gameObject.SetActive(false);
        _hitboxRoot.HitboxRootActive = false;
        _characterMovementHandler.SetCharacterControllerEnabled(false);

        Instantiate(DeathPlayerPrefab, transform.position, Quaternion.identity);
    }

    private void OnRevive()
    {
        Debug.Log("revive");

        if (Object.HasInputAuthority)
        {
            UIOnHitImg.color = new Color(0, 0, 0, 0);
        }

        PlayerModel.gameObject.SetActive(true);
        _hitboxRoot.HitboxRootActive = true;
        _characterMovementHandler.SetCharacterControllerEnabled(true);

    }
}
