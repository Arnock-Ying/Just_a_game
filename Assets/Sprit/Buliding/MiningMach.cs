using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;
using Manager;

public class MiningMach : BaseBuild
{
    [SerializeField]
    private Vector2Int[] MiningRange = new Vector2Int[1];
    [SerializeField]
    private string[] Mines = new string[1];
    [SerializeField]//暂时序列化可视，debug
    private List<Mineral> minlist = new List<Mineral>();
    [SerializeField]
    private int dig_num = 1;
    private float timer = 0;
    private const float delayTime = 1;
    private bool noinit = true;

    private void Start()
    {
        MapManager.SetBuild(transform.position, size, this);
        invent = new(maxInvent);
    }

    private void Update()
    {
        //应急初始化
        if (noinit)
        {
            noinit = false;
            FindMineral();
        }

        timer += Time.deltaTime;
        if (timer >= delayTime)
        {
            Mining();
            timer = 0;
            //Debug.Log(invent.GetLog());
        }
        debug = "内存wood数量:" + invent.Contains("wood");//
    }

    protected virtual void FindMineral()
    {
        foreach (Vector2Int v in MiningRange)
        {
            Mineral m = MapManager.GetBlock(Mathf.FloorToInt(transform.position.x) + v.x, Mathf.FloorToInt(transform.position.y) + v.y) as Mineral;
            if (m)
            {
                foreach (string id in Mines)
                    if (id == m.Item.id)
                    {
                        minlist.Add(m);
                        break;
                    }
            }
        }
    }

    protected virtual void Mining()
    {
        if (minlist.Count == 0)
        {
            return;
        }
        for (int i = 0; i < minlist.Count; ++i)
        {
            Mineral m = minlist[i];
            if (m.Item.count > dig_num)
            {
                if (invent.Input(m.Item.id, dig_num))
                    m.Item.count -= dig_num;
            }
            else if (m.Item.count > 0)
            {
                if (invent.Input(m.Item.id, m.Item.count))
                    m.Item.count = 0;
            }
        }
    }

}
