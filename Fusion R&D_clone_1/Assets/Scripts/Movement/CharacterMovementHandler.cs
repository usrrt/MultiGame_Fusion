using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class CharacterMovementHandler : NetworkBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    //Vector2 _viewInput;

    //// rotation
    //float _camRotationX = 0f;

    // 재탄생 요청
    bool _isRespawnRequest = false;

    NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
    HPHandler _hPHandler;

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        _hPHandler = GetComponent<HPHandler>();
    }

    void Start()
    {

    }

    private void Update()
    {
        //_camRotationX += _viewInput.y * Time.deltaTime * _networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
        //_camRotationX = Mathf.Clamp(_camRotationX, -90, 90);

        //_localCam.transform.localRotation = Quaternion.Euler(_camRotationX, 0, 0);
    }

    public override void FixedUpdateNetwork()
    {
        // 로컬플레이어의 상태만 가져온다
        if (Object.HasStateAuthority)
        {
            if (_isRespawnRequest)
            {
                Respawn();
                return;
            }

            // 플레이어가 죽으면 움직일수없다
            if (_hPHandler.IsDead)
            {
                return;
            }

        }

        // 네트워크로부터 입력 가져오기
        if (GetInput(out NetworkInputData networkInputData))
        {
            //// rotate the view
            //_networkCharacterControllerPrototypeCustom.Rotate(networkInputData.RotaionInput);

            // 클라이언트의 aim vector에 따라 회전
            transform.forward = networkInputData.AimForwardVector;

            // 캐릭터가 x축으로 기울이는것 방지
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            // move
            Vector3 moveDirection = transform.forward * networkInputData.MovementInput.y + transform.right * networkInputData.MovementInput.x;
            moveDirection.Normalize();

            _networkCharacterControllerPrototypeCustom.Move(moveDirection);

            // jump
            if (networkInputData.IsJumpPressed)
            {
                _networkCharacterControllerPrototypeCustom.Jump();
            }

            // 낭떠러지로 낙하시 리스폰
            CheckFallRespawn();
        }
    }

    public void RequestRespawn()
    {
        _isRespawnRequest = true;
    }

    private void Respawn()
    {
        _networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());

        _hPHandler.OnRespawn();

        _isRespawnRequest = false;
    }

    private void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            // transform.position = Utils.GetRandomSpawnPoint();
            if (Object.HasStateAuthority)
            {
                Debug.Log($"{gameObject.name}낭떠러지로 떨어지는중");

                Respawn();
            }
        }
    }

    //public void SetViewInputVector(Vector2 viewInput)
    //{
    //    _viewInput = viewInput;
    //}

    // 캐릭터 사망시 캐릭터컨트롤러 꺼주는 과정
    public void SetCharacterControllerEnabled(bool isEnable)
    {
        _networkCharacterControllerPrototypeCustom.Controller.enabled = isEnable;
    }
}
