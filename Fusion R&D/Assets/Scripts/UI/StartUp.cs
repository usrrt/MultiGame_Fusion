using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp
{
    // runtime initialize on load�Լ��� ����Ƽ�� ���������� ������ �ν��Ͻ�ȭ�� �����Ҽ��ִ� ���� ����� �ϳ�(����Ƽ ������ �����Ǿ���� ���ӿ�����Ʈ�� GameManager, ���� �ε�Ǳ����� �����ؾ��� ������̵� �� ��������)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InstantiatePrefabs()
    {
        Debug.Log("---������Ʈ�� �ν��Ͻ�ȭ �ϴ���---");

        // ���ӿ�����Ʈ �迭�� �ν��Ͻ��� �����յ� ���
        GameObject[] prefabsToInstantiate = Resources.LoadAll<GameObject>("InstantiateOnLoad/");

        foreach (GameObject prefab in prefabsToInstantiate)
        {
            Debug.Log($"Creating : {prefab.name}");

            GameObject.Instantiate(prefab);
        }
        
        Debug.Log("---�ν��Ͻ�ȭ ����---");
    }
}