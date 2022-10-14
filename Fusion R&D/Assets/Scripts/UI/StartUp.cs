using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp
{
    // runtime initialize on load함수는 유니티가 켜지기전에 생성과 인스턴스화를 실행할수있는 좋은 방법중 하나(유니티 실행전 생성되어야할 게임오브젝트나 GameManager, 씬이 로드되기전에 존재해야할 어느것이든 다 들어갈수있음)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InstantiatePrefabs()
    {
        Debug.Log("---오브젝트들 인스턴스화 하는중---");

        // 게임오브젝트 배열에 인스턴스한 프리팹들 담기
        GameObject[] prefabsToInstantiate = Resources.LoadAll<GameObject>("InstantiateOnLoad/");

        foreach (GameObject prefab in prefabsToInstantiate)
        {
            Debug.Log($"Creating : {prefab.name}");

            GameObject.Instantiate(prefab);
        }
        
        Debug.Log("---인스턴스화 끝남---");
    }
}