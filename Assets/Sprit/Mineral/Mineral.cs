using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;
using Manager;

public class Mineral : Block
{
    public Vector2Int size = new Vector2Int(1, 1);
    private Item item;
    public string id;
    public int count;

    private void Start()
    {
        MapManager.SetBlock(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), this);
        //InitItem(id, count);
        //Debug.Log(item.id);
        //Debug.Log(item.count);
    }

    //private void InitItem(string id, int count)
    //{
    //    item.id = id;
    //    item.count=count;
    //}

}
