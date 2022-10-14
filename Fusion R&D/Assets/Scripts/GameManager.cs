using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    // �ڸ��ϰԵ� ���ӸŴ����� �̱������� �����Ѵ�
    // ���⼭�� ���Ͼ������� �����ϴ� �̱����ڵ带 ����� => ��Ƽȯ���� �̱����� �ٸ������� �����Ҽ������Ƿ�

    // static���� ������ instance �ֳ��ϸ� �ٸ� ��ũ��Ʈ�� �����Ҽ��ְ��ϱ�����
    public static GameManager Instance = null;

    // connetion token�� byte �迭
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
        // ��ū�� ��ȿ���� Ȯ��, ���ٸ� �ϳ� ������
        if (connectionToken == null)
        {
            connectionToken = ConnectionTokenUtils.NewToken();
            Debug.Log($"�÷��̾� ���� ��ū {ConnectionTokenUtils.HashToken(connectionToken)}");
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
