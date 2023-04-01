using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;
using Manager;

namespace Logist
{
	public class LogistPipe : Block
	{
		[SerializeField] private Sprite[] pipeImage;

		SpriteRenderer spr;

		int con_num = 0;    //连接数量
		Block[] findbuilding = new Block[4];//上右下左是否有建筑
		int[] step = { 0, -1, 0, 1, 1, 0, -1, 0 };//上右下左

		private void Start()
		{
			spr = GetComponent<SpriteRenderer>();
			FindBuilding(false);
			ChooseMap();
		}

		private void FindBuilding(bool isstand)
		{
			Vector2Int pos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
			//Debug.Log(pos);
			for (int i = 0; i < 4; i++)
			{
				var block = MapManager.GetBlock(pos.x + step[i], pos.y + step[i + 4]);
				//Debug.Log(block);
				if (block != null && block.gameObject.tag == "Building")
				{
					findbuilding[i] = block;
					con_num++;
					if (!isstand && block is LogistPipe) ((LogistPipe)block).UpdateMap();
				}
			}
		}

		private void ChooseMap()
		{
			//加载con_num对应贴图
			if (con_num == 2 && !(findbuilding[0] ^ findbuilding[2]))
				spr.sprite = pipeImage[5];
			else
				spr.sprite = pipeImage[con_num];
			//旋转
			//Debug.Log(con_num);
			if (con_num == 0 || con_num == 4) return;
			if (con_num == 2 && !(findbuilding[0] ^ findbuilding[2]))
			{
				if (findbuilding[0]) transform.eulerAngles = new Vector3(0, 0, 90);
			}
			else if (con_num == 1)
			{
				int cnt = 0;
				while (!findbuilding[cnt]) cnt++;
				transform.eulerAngles = new Vector3(0, 0, 90 * (cnt - 1));

			}
			else if (con_num == 2)
			{
				int cnt = 0;
				while (!findbuilding[cnt] || !findbuilding[(cnt + 1) & 3]) cnt++;
				transform.eulerAngles = new Vector3(0, 0, 90 * (cnt));
			}
			else if (con_num == 3)
			{
				int cnt = 0;
				while (findbuilding[cnt]) cnt++;
				transform.eulerAngles = new Vector3(0, 0, 90 * (cnt + 2));
			}
		}

		private void UpdateMap()
		{
			con_num = 0;
			for (int i = 0; i < 4; i++) findbuilding[i] = null;
			FindBuilding(true);
			ChooseMap();
		}
	}
}