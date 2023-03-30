using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;
using Manager;

public class Logging_station : BaseBuild
{
    List<Mineral> minlist = new List<Mineral>();
    public int dig_num = 1;
    private float timer = 0;
    private float delayTime = 1;

    private void Start()
    {
        hp = 6;
        FindMineral();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= delayTime)
        {
            Mining(dig_num);
            timer = 0;
        }
    }

    protected virtual void FindMineral()
    {
        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        for (int i = pos.x - 1; i <= pos.x + 1; ++i)
        {
            for (int j = pos.y - 1; j <= pos.y + 1; ++j)
            {
                //if (i == pos.x && j == pos.y) continue;
                Mineral mine = MapManager.GetBlock(i, j) as Mineral;
                if (mine != null)
                {
                    minlist.Add(mine);
                }
            }

        }
    }

    protected virtual void Mining(int dig_num)
    {
        if (minlist.Count == 0)
        {
            return;
        }
        for (int i = 0; i < minlist.Count; ++i)
        {
            Mineral mine = minlist[i];
            if (mine.count > dig_num)
            {

            }
            else if (dig_num >= mine.count && mine.count > 0)
            {

            }
            else
            {

            }
        }
    }

}
