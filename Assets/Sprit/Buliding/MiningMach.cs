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
	private float delayTime = 1;
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
	}

	protected virtual void FindMineral()
	{
		foreach (Vector2Int v in MiningRange)
		{
			Mineral m = MapManager.GetBlock(Mathf.FloorToInt(transform.position.x) + v.x, Mathf.FloorToInt(transform.position.y) + v.y) as Mineral;
			if (m)
			{
				foreach (string id in Mines)
					if (id == m.id)
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
			Mineral mine = minlist[i];
			if (mine.count > dig_num)
			{
				if (invent.Input(mine.id, dig_num))
					mine.count -= dig_num;
			}
			else if (mine.count > 0)
			{
				if (invent.Input(mine.id, mine.count))
					mine.count = 0;
			}
		}
	}

}
