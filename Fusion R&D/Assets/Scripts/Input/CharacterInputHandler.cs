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

    #region Other Components

    //CharacterMovementHandler _movementHandler;
    LocalCameraHandler _localCameraHandler;

    #endregion

    private void Awake()
    {
        //_movementHandler = GetComponent<CharacterMovementHandler>();
        _localCameraHandler = GetComponentInChildren<LocalCameraHandler>();

    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // view input
        _viewInputVector.x = Input.GetAxis("Mouse X");
        _viewInputVector.y = Input.GetAxis("Mouse Y") * -1; // 위아래 반대로 뒤집기위해 -1곱함

        //_movementHandler.SetViewInputVector(_viewInputVector);

        // move input
        _moveInputVector.x = Input.GetAxis("Horizontal");
        _moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            _isJumpBtnPressed = true;
        }
        
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

        // 상태를 읽으면 값 초기화
        _isJumpBtnPressed = false;


        return networkInputData;
    }
}
