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
        // ��Ʈ��ũ�κ��� �Է� ��������
        if (GetInput(out NetworkInputData networkInputData))
        {
            //// rotate the view
            //_networkCharacterControllerPrototypeCustom.Rotate(networkInputData.RotaionInput);

            // Ŭ���̾�Ʈ�� aim vector�� ���� ȸ��
            transform.forward = networkInputData.AimForwardVector;

            // ĳ���Ͱ� x������ ����̴°� ����
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

            // ���������� ���Ͻ� ������
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