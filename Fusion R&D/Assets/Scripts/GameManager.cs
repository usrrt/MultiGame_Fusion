using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    // 자명하게도 게임매니저는 싱글톤으로 생성한다
    // 여기서는 독일아저씨가 정의하는 싱글톤코드를 사용함 => 멀티환경의 싱글톤은 다른구조가 존재할수있으므로

    // static으로 정의한 instance 왜냐하면 다른 스크립트에 접근할수있게하기위해
    public static GameManager Instance = null;

    // connetion token용 byte 배열
    byte[] connectionToken;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 토큰이 유효한지 확인, 없다면 하나 생성함
        if (connectionToken == null)
        {
            connectionToken = ConnectionTokenUtils.NewToken();
            Debug.Log($"플레이어 연결 토큰 {ConnectionTokenUtils.HashToken(connectionToken)}");
        }
    }

    public void SetConnectionToken(byte[] connectionToken)
    {
        this.connectionToken = connectionToken;
    }

    public byte[] GetConnectionToken()
    {
        return connectionToken;
    }

}
