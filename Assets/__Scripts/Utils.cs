using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    //=========================================Функйции для работы с материалломи==========================\\
    static public Material[] GetMaterials(GameObject go)
    {
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();

        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends) 
        {
            mats.Add(rend.material);
        }
        return (mats.ToArray());
    }
}
