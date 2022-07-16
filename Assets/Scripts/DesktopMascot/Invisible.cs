using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アタッチされたオブジェクトは見かけ上非表示にする
/// </summary>
public class Invisible : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
    }
}
