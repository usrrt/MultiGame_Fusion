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

    NetworkCharacterControllerPrototypeCustom _networkCharacterControllerPrototypeCustom;
    Camera _localCam;

    private void Awake()
    {
        _networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        _localCam = GetComponentInChildren<Camera>();
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

    private void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            transform.position = Utils.GetRandomSpawnPoint();
        }
    }

    //public void SetViewInputVector(Vector2 viewInput)
    //{
    //    _viewInput = viewInput;
    //}
}
