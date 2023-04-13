using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;
using Manager;

public class Mineral : Block
{
    private Item item = new();
    public Item Item { get => item; }
    //[SerializeField]
    public string id;
    //[SerializeField]
    public int count;

    private void Start()
    {
        MapManager.SetBuild(transform.position, size, this);
        InitItem(id, count);
        //Debug.Log(item.id);
        //Debug.Log(item.count);
    }

    private void Update()
    {
        id = item.id;
        count = item.count;
    }

    private void InitItem(string id, int count)
    {
        item.id = id;
        item.count = count;
    }

}
