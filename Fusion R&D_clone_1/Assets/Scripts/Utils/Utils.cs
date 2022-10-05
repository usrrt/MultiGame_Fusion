using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################
    
    /// <summary>
    /// ���� ��ġ ����
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-10, 10), 4, Random.Range(-10, 10));
    }

    public static void SetRenderLayerInChildren(Transform transform, int layerNum)
    {
        foreach (Transform trans in transform.GetComponentInChildren<Transform>(true))
        {
            if (trans.CompareTag("IgnoreLayerChange"))
            {
                continue;
            }

            trans.gameObject.layer = layerNum;

        }
    }
    
}
