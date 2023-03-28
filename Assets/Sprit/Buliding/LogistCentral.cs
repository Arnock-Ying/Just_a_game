using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameBase;

public class LogistCentral : BaseBuild
{
    private LogistNet managedlogist;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        var col = gameObject.GetComponent<BoxCollider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

    }

}
