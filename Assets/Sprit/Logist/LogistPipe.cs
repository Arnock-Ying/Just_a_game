using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameBase;
using Manager;

namespace Logist
{
	public class LogistPipe : Block
	{
		SpriteRenderer spr;
		LogistNet net;

		//debug
		[SerializeField]
		string debug;

		Router router = null;
		ushort[] temp_rout = null, inter_rout = null;
		Dircation temp_dir = 0, inter_dir = 0;

		int con_num = 0;    //��������
		Block[] findbuilding = new Block[4];//���������Ƿ��н���

		private void Start()
		{
			spr = GetComponent<SpriteRenderer>();
			FindBuilding(false);
		}

		private void FixedUpdate()
		{
			if (router != null)
				if (temp_rout != null && inter_rout != null)
				{
					if (temp_rout != null) relayRoute(temp_rout, temp_dir);
					temp_rout = inter_rout;
					temp_dir = inter_dir;
				}
			debug = router == null ? "null" : "router";
		}

		public void setRelayRoute(ushort[] rout, Dircation dir)
		{
			inter_rout = rout; inter_dir = dir;
		}
		private void relayRoute(ushort[] rout, Dircation dir)
		{
			if (con_num < 2) return;
			if (router == null)
			{
				for (int i = 0; i < 256; ++i)
					if (rout[i] != 0) rout[i] += 4;
				for (int i = 0; i < 4; ++i)
					if ((int)dir != i && findbuilding[i] != null)
					{
						if (findbuilding[i] is LogistPipe pipe) pipe.setRelayRoute(rout, (Dircation)i);
					}
			}
			else
			{
				bool back = router.ChangeRoute(rout, dir);
				for (int i = 0; i < 4; ++i)
					if ((((int)dir != i) ^ back) && findbuilding[i] != null)
					{
						if (findbuilding[i] is LogistPipe pipe) pipe.setRelayRoute(router.IpTable, (Dircation)i);
					}
				GC.Collect();//�ֶ��������������ͷ�һ��
			}
		}

		public void FindBuilding(bool isstand)
		{
			Vector2Int pos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
			//Debug.Log(pos);
			con_num = 0;
			for (int i = 0; i < 4; i++)
			{
				int[] step = { -1, 1, 0, 0, 0, 0, 1, -1 };
				var block = MapManager.GetBlock(pos.x + step[i], pos.y + step[i + 4]);
				//Debug.Log(block);
				
				if (block != null && block.gameObject.CompareTag("Building"))
				{
					con_num++;
					if (block is LogistPipe pipe)
					{
						findbuilding[i] = pipe;
						if (!isstand)
							pipe.UpdateMap();
					}
					else if (block is BaseBuild build)
					{
						if (findbuilding[i] is not InterFace nowinter || nowinter.block != build)
						{
							InterFace inter = Instantiate(Resources.Load<GameObject>("Pipe/InterFace")).GetComponent<InterFace>();
							inter.gameObject.transform.position = new Vector3(transform.position.x + step[i] * 0.5f, transform.position.y + step[i + 4] * 0.5f, -1.5f);
							build.InterFaces.Add(inter);
							findbuilding[i] = inter;
						}

					}
					else if (block is null)
					{
						if (findbuilding[i] is InterFace inter)
						{
							inter.Remove();
							Destroy(inter.gameObject);
						}
						findbuilding[i] = null;
					}
				}

			}
			//foreach (var i in findbuilding)
			//	Debug.Log(i);

			if (!(con_num == 2 && !(findbuilding[0] ^ findbuilding[1])) && con_num >= 2)
			{
				if (router == null)
				{
					router = new(this);
				}
			}
			else
			{
				router = null;
				GC.Collect();//�ֶ��������������ͷ�һ��
			}
			ChooseImage();
		}

		public void ChooseImage()
		{
			//����con_num��Ӧ��ͼ
			if (con_num == 2 && !(findbuilding[0] ^ findbuilding[1]))
				spr.sprite = LogistManager.Instend.PipeImage[5];
			else
				spr.sprite = LogistManager.Instend.PipeImage[con_num];
			//��ת
			//Debug.Log(con_num);
			if (con_num == 0 || con_num == 4) return;
			if (con_num == 2 && !(findbuilding[0] ^ findbuilding[1]))
			{
				if (findbuilding[2]) transform.eulerAngles = new Vector3(0, 0, 90);
			}
			else
			{
				int[] und = { 2, 0, 3, 1 };
				int cnt = 0;
				for (int i = 0; (findbuilding[und[i]]) || !(findbuilding[und[(i + 1) % 4]]); ++i)
				{
					//Debug.Log(cnt + "--(" + und[i] + "," + und[(i + 1) % 4] + ")" + (findbuilding[und[i]] != null) + ":" + (findbuilding[und[(i + 1) % 4]] != null));
					++cnt;
				}

				if (con_num == 1) transform.eulerAngles = new Vector3(0, 0, 90 * (cnt));
				else if (con_num == 2) transform.eulerAngles = new Vector3(0, 0, 90 * (cnt + 1));
				else if (con_num == 3) transform.eulerAngles = new Vector3(0, 0, 90 * (cnt + 2));
			}
		}

		private void UpdateMap()
		{
			con_num = 0;
			for (int i = 0; i < 4; i++) findbuilding[i] = null;
			FindBuilding(true);
		}
	}
}