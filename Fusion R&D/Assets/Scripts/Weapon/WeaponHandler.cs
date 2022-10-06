using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class WeaponHandler : NetworkBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    public ParticleSystem FireParticle;
    public Transform AimPoint;
    public LayerMask CollisionLayers;

    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool IsFiring { get; set; }

    float _lastTimeFired = 0f;

    #region Other Components

    HPHandler _hPhandler;
    NetworkPlayer _player;

    #endregion

    private void Awake()
    {
        _hPhandler = GetComponent<HPHandler>();
        _player = GetComponent<NetworkPlayer>();
    }

    void Start()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (_hPhandler.IsDead)
            {
                return;
            }
        }

        // network로부터 input받기
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.IsFireBtnPressed)
            {
                Fire(networkInputData.AimForwardVector);
                //Fire(AimPoint.forward);
            }
        }
    }

    private void Fire(Vector3 aimForwardVec)
    {

        // 연사 비율
        if (Time.time - _lastTimeFired < 0.15f)
        {
            return;
        }

        StartCoroutine(FireEffect());

        // 처음엔 HitOptions의 IncludePhysX를 썼으나 
        // 호스트가아닌 다른 플레이어의 경우 뒤로가면서 총을쏘면 자신이 그 총에 맞는 경우가 생김
        // 퓨전의 새로운 버전에선 이런 현상을 해결하기 위해 IgnoreInputAuthority라는 새로운 프로퍼티가 생김
        // 맞는대상에서 자기자신을 제외할수있음
        Runner.LagCompensation.Raycast(AimPoint.position, aimForwardVec, 100f, Object.InputAuthority, out var hitInfo, CollisionLayers, HitOptions.IgnoreInputAuthority);

        float hitDistance = 100f;
        bool isHitOtherPlayer = false;

        if (hitInfo.Distance > 0)
        {
            hitDistance = hitInfo.Distance;
        }

        if (hitInfo.Hitbox != null)
        {
            Debug.Log($"{transform.name} hitbox {hitInfo.Hitbox.transform.root.name}");

            if (Object.HasStateAuthority)
            {
                hitInfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage(_player.Nickname.ToString());
            }

            isHitOtherPlayer = true;

        }
        else if (hitInfo.Collider != null)
        {
            Debug.Log($"{transform.name} physX {hitInfo.Collider.transform.name}");
        }

        if (isHitOtherPlayer)
        {
            Debug.DrawRay(AimPoint.position, aimForwardVec * hitDistance, Color.red, .5f);
        }
        else
        {
            Debug.DrawRay(AimPoint.position, aimForwardVec * hitDistance, Color.blue, .5f);

        }

        _lastTimeFired = Time.time;
    }

    private static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        // static 함수내에서 프로퍼티, 함수를 사용하고자 한다면 -> Behaviour사용
        //Debug.Log($"{Time.time} onfire {changed.Behaviour.IsFiring}");

        bool isFiringCurrent = changed.Behaviour.IsFiring;

        // 이전의 value를 load한다
        changed.LoadOld();


        bool isFiringOld = changed.Behaviour.IsFiring;

        if (isFiringCurrent && !isFiringOld)
        {
            changed.Behaviour.OnFireRemote();
        }
    }

    private void OnFireRemote()
    {

    }

    IEnumerator FireEffect()
    {
        IsFiring = true;

        FireParticle.Play();

        yield return new WaitForSeconds(0.09f);

        IsFiring = false;
    }
}
