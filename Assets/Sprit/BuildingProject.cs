using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameBase;

///<summary>
///½¨Öþ¹æ»®
///</summary>
public class BuildingProject : Block
{

    private Vector3 size;
    BuildingProject(GameObject prefab)
    {
        size = prefab.transform.GetComponent<Collider>().bounds.size;
        Debug.Log(size);
    }
    BuildingProject() { }

    private void Start()
    {

    }

    private void Update()
    {

    }

}

