using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameBase;

public class Base : BaseBuild
{
    void Start()
    {
        var col = gameObject.GetComponent<BoxCollider>();
    }

    void Update()
    {
        
    }
}
