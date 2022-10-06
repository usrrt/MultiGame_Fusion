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

    // queue를 사용해 들어온 순서대로 메시지 관리
    // 처음에 들어온 메시지는 큐 크기를 넘어가면 제일먼저 제거된다
    Queue messageQueue = new Queue();
    
    void Start()
    {
        
    }

    public void OnGameMessageReceived(string message)
    {
        Debug.Log("in game message " + message);

        // 큐안에 메시지 넣기
        messageQueue.Enqueue(message);


        // 설정된 범위를 넘어가면 처음메시지 지워주기
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
