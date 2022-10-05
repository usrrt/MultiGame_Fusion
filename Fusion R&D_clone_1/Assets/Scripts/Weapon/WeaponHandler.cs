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

    #endregion

    private void Awake()
    {
        _hPhandler = GetComponent<HPHandler>();
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

        Runner.LagCompensation.Raycast(AimPoint.position, aimForwardVec, 100f, Object.InputAuthority, out var hitInfo, CollisionLayers, HitOptions.IncludePhysX);

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
                hitInfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage();
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
