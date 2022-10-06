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
    // Ʃ�丮�󿡼� ������Ʈ�� instantiate�� ����� �����ϰ�������
    // ������ƮǮ���� ����� ��ƼŬ �������°� �����ս� ��� �������ش�
    public GameObject DeathPlayerPrefab; 

    #region Other Components

    // �÷��̾ �ѿ� �ι��� �´°� ����
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

        // �ֱٿ� �������¶��
        if (isDeathCurrent)
        {
            // ���� �޼ҵ� ����
            changed.Behaviour.OnDeath();
        }
        // �ֱٿ� ����ְ� ���ſ� �������¿��ٸ�
        else if (!isDeathCurrent && isDeadOld)
        {
            // �һ� �޼ҵ� ����
            changed.Behaviour.OnRevive();
        }
    }

    private static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log("HP value " + changed.Behaviour.HP);

        byte newHP = changed.Behaviour.HP;

        changed.LoadOld();
       

        byte oldHP = changed.Behaviour.HP;

        // HP��ȭ üũ -> �ֽ�hp�� ������hp���� �۴� -> hp�� ���ҵȻ�Ȳ
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

    // ���� ������ ���ؼ��� �����
    public void OnTakeDamage(string damageCauseByPlayerNickname)
    {
        // ����������� ������ ����
        if (IsDead)
        {
            return;
        }

        HP -= 1;

        Debug.Log($"{transform.name} ���� ü�� : {HP}");

        if (HP <= 0)
        {
            // damageCauseByPlayerNickname => ���λ�� => ���� �÷��̾�
            // _networkPlayer.Nickname.ToString() => ������� => ����Ʈ �÷��̾� 
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
