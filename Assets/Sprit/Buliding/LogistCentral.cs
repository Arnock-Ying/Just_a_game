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
        //Color32 red = new Color32(255, 0, 0, 255);
        //Color32 green = new Color32(0, 255, 0, 255);
    }

    void Update()
    {

    }

}
