using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;
using Manager;

public class Smelter : ProductionBuilding
{
    void Start()
    {
        MapManager.SetBuild(transform.position, size, this);
        base.Start();
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        
    }
}
