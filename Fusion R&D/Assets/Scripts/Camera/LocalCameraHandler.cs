using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    // 네트워크상태가 안좋을시 시야이동이 끊기는걸 방지하기위한 스크립트
    // Update함수사용할예정
    // 캐릭터와 연관없음 -> 캐릭터는 네트워크이므로

    public Transform CameraAnchorPoint;

    public Camera LocalCam;
    NetworkCharacterControllerPrototypeCustom _characterController;
    // rotation
    float _cameraRotationX = 0;
    float _cameraRotationY = 0;

    // view
    Vector2 _viewInput;

    private void Awake()
    {
        LocalCam = GetComponent<Camera>();
        _characterController = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }

    void Start()
    {
        //// 로컬카메라 활성화되어있다면 카메라를 부모로부터 분리
        //if (_localCam.enabled)
        //{
        //    _localCam.transform.parent = null;
        //}


    }

    private void LateUpdate()
    {
        if (CameraAnchorPoint == null)
        {
            return;
        }

        if (!LocalCam.enabled)
        {
            return;
        }

        // 카메라를 플레이어 위치로 이동
        LocalCam.transform.position = CameraAnchorPoint.position;

        // rotation 계산
        _cameraRotationX += _viewInput.y * Time.deltaTime * _characterController.viewUpDownRotationSpeed;
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90, 90);

        _cameraRotationY += _viewInput.x * Time.deltaTime * _characterController.rotationSpeed;

        // rotaion 적용
        LocalCam.transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0f);
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this._viewInput = viewInput;
    }
}
