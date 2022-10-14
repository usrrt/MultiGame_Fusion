using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    // ��Ʈ��ũ���°� �������� �þ��̵��� ����°� �����ϱ����� ��ũ��Ʈ
    // Update�Լ�����ҿ���
    // ĳ���Ϳ� �������� -> ĳ���ʹ� ��Ʈ��ũ�̹Ƿ�

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
        //// ����ī�޶� Ȱ��ȭ�Ǿ��ִٸ� ī�޶� �θ�κ��� �и�
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

        // ī�޶� �÷��̾� ��ġ�� �̵�
        LocalCam.transform.position = CameraAnchorPoint.position;

        // rotation ���
        _cameraRotationX += _viewInput.y * Time.deltaTime * _characterController.viewUpDownRotationSpeed;
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90, 90);

        _cameraRotationY += _viewInput.x * Time.deltaTime * _characterController.rotationSpeed;

        // rotaion ����
        LocalCam.transform.rotation = Quaternion.Euler(_cameraRotationX, _cameraRotationY, 0f);
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this._viewInput = viewInput;
    }
}
