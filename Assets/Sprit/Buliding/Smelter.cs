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
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        
    }
}
