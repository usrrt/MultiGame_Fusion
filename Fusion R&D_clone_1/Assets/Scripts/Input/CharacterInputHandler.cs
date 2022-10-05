using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    Vector2 _moveInputVector = Vector2.zero;
    Vector2 _viewInputVector = Vector2.zero;
    bool _isJumpBtnPressed = false;
    bool _isFireBtnPressed = false;

    #region Other Components

    CharacterMovementHandler _movementHandler;
    LocalCameraHandler _localCameraHandler;
   

    #endregion

    private void Awake()
    {
        _movementHandler = GetComponent<CharacterMovementHandler>();
        _localCameraHandler = GetComponentInChildren<LocalCameraHandler>();

    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // �÷��̾� ��Ʈ�� ���� Ȯ��
        if (!_movementHandler.Object.HasInputAuthority)
        {
            return;
        }

        // view input
        _viewInputVector.x = Input.GetAxis("Mouse X");
        _viewInputVector.y = Input.GetAxis("Mouse Y") * -1; // ���Ʒ� �ݴ�� ���������� -1����

        //_movementHandler.SetViewInputVector(_viewInputVector);

        // move input
        _moveInputVector.x = Input.GetAxis("Horizontal");
        _moveInputVector.y = Input.GetAxis("Vertical");

        // jump input
        if (Input.GetButtonDown("Jump"))
        {
            _isJumpBtnPressed = true;
        }

        // fire input
        if (Input.GetButtonDown("Fire1"))
        {
            _isFireBtnPressed = true;
        }
        
        // set view
        _localCameraHandler.SetViewInputVector(_viewInputVector);

    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //// view data
        //networkInputData.RotaionInput = _viewInputVector.x;

        // aim data
        networkInputData.AimForwardVector = _localCameraHandler.transform.forward;

        // move data
        networkInputData.MovementInput = _moveInputVector;

        // jump data
        networkInputData.IsJumpPressed = _isJumpBtnPressed;

        // fire data
        networkInputData.IsFireBtnPressed = _isFireBtnPressed;

        // ���¸� ������ �� �ʱ�ȭ
        _isJumpBtnPressed = false;
        _isFireBtnPressed=false;


        return networkInputData;
    }
}
