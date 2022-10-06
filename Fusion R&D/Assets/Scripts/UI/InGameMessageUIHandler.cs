using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameMessageUIHandler : MonoBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    public TextMeshProUGUI[] TextMeshProUGUIs;

    // queue�� ����� ���� ������� �޽��� ����
    // ó���� ���� �޽����� ť ũ�⸦ �Ѿ�� ���ϸ��� ���ŵȴ�
    Queue messageQueue = new Queue();
    
    void Start()
    {
        
    }

    public void OnGameMessageReceived(string message)
    {
        Debug.Log("in game message " + message);

        // ť�ȿ� �޽��� �ֱ�
        messageQueue.Enqueue(message);


        // ������ ������ �Ѿ�� ó���޽��� �����ֱ�
        if (messageQueue.Count > 3)
        {
            messageQueue.Dequeue();
        }


        int queueIdx = 0;
        foreach (string messageInQueue in messageQueue)
        {
            TextMeshProUGUIs[queueIdx].text = messageInQueue;
            queueIdx++;
        }


    }
}
