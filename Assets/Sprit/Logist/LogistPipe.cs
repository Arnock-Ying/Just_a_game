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
		[SerializeField]
		SpriteRenderer spr;
		LogistNet net;

		//debug
		[SerializeField]
		string debug;

		Router router = null;
		ushort[] temp_rout = null, inter_rout = null;
		Dircation temp_dir = 0, inter_dir = 0;

		int con_num = 0;    //连接数量
		Block[] findbuilding = new Block[4];//上下左右是否有建筑

		private LogistNet parentLogist = null;
		public override LogistNet ParentLogist { get => parentLogist; set => parentLogist = value; }

		private void Start()
		{
			BuildPipe(true);
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
					if (((int)dir != i) && findbuilding[i] != null)
					{
						if (findbuilding[i] is LogistPipe pipe) pipe.setRelayRoute(router.IpTable, (Dircation)i);
					}
					else if (back && findbuilding[i] != null)
					{
						if (findbuilding[i] is LogistPipe pipe) pipe.setRelayRoute(router.IpTable, (Dircation)i);
					}
				GC.Collect();//手动让垃圾回收器释放一下
			}
		}

		public void BuildPipe(bool rebuild)
		{
			Vector2Int pos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
			//Debug.Log(pos);
			con_num = 0;
			for (int i = 0; i < 4; i++)
			{
				int[] step = { -1, 1, 0, 0, 0, 0, 1, -1 };
				var block = MapManager.GetBlock(pos.x + step[i], pos.y + step[i + 4]);
				//Debug.Log(block);

				if (block != null)
				{
					if (block is BaseBuild build)
					{
						con_num++;
						if (findbuilding[i] is not InterFace nowinter || nowinter.build != build)
						{
							InterFace inter = Instantiate(Resources.Load<GameObject>("Pipe/InterFace")).GetComponent<InterFace>();
							inter.gameObject.transform.position = new Vector3(transform.position.x + step[i] * 0.5f, transform.position.y + step[i + 4] * 0.5f, -1.5f);
							build.InterFaces.Add(inter);
							inter.build = build;
							inter.pipe = this;
							findbuilding[i] = inter;
						}
					}
					if (block is LogistPipe pipe)
					{
						if (findbuilding[i] is InterFace inter)
							inter.DestroyBlock();

						con_num++;
						findbuilding[i] = pipe;

						if (rebuild)
							pipe.BuildPipe(false);
					}
				}
				else
				{
					if (findbuilding[i] is InterFace inter)
					{
						inter.DestroyBlock();
					}
					findbuilding[i] = null;
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
				GC.Collect();//手动让垃圾回收器释放一下
			}
			ChooseImage();

			if (!rebuild)
			{

				//处理物流网络信息
				for (int i = 0; i < 4; ++i)
				{
					if (findbuilding[i] == null) continue;
					else if (findbuilding[i] is LogistPipe pipe)
					{
						LogistNet tempnet = null;
						//Debug.Log($"{(parentLogist == null ? "null" : "logist")},{(block.ParentLogist == null ? "null" : "logist")}");
						try
						{
							tempnet = LogistNet.Marge(parentLogist, pipe.ParentLogist);
						}
						catch (System.Exception exc)
						{
							Debug.LogError(exc.Message);
						}
						finally
						{
							parentLogist = pipe.ParentLogist = tempnet;
						}
					}
					else if (findbuilding[i] is InterFace inter)
					{
						if (parentLogist == null) parentLogist = new LogistNet();
						ushort[] vs = null;
						if (parentLogist.manager == null || inter.build is LogistCentral)
						{
							if (parentLogist.SetManager(inter.build))
							{
								vs = Router.MakeTable(0, (Dircation)i);
							}
							else
							{
								Debug.Log("set massgae false");
							}
						}
						else
						{
							if (parentLogist.SetIp(inter.build))
							{
								vs = Router.MakeTable(inter.build.Ip, (Dircation)i);
								Debug.Log("set ip false");
							}
						}
						if (vs != null)
							setRelayRoute(vs, (Dircation)(i ^ 1));
					}
				}
			}
		}

		public static KeyValuePair<Sprite, float> GetImageAndAngles(Vector3 position)
		{
			Vector2Int pos = new(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
			//Debug.Log(pos);
			int con_num = 0;
			bool[] findbuilding = new bool[4];
			for (int i = 0; i < 4; i++)
			{
				findbuilding[i] = false;
				int[] step = { -1, 1, 0, 0, 0, 0, 1, -1 };

				var block = MapManager.GetBlock(pos.x + step[i], pos.y + step[i + 4]);
				if (block is BaseBuild || block is LogistPipe)
				{
					++con_num;
					findbuilding[i] = true;
				}
			}
			Sprite sprite;
			//加载con_num对应贴图
			if (con_num == 2 && !(findbuilding[0] ^ findbuilding[1]))
				sprite = LogistManager.Instend.PipeImage[5];
			else
				sprite = LogistManager.Instend.PipeImage[con_num];

			//旋转
			//Debug.Log(cnt + "--(" + und[i] + "," + und[(i + 1) % 4] + ")" + (findbuilding[und[i]] != null) + ":" + (findbuilding[und[(i + 1) % 4]] != null));
			float ans = 0;
			if (con_num == 0 || con_num == 4) ans = 0;
			else if (con_num == 2 && !(findbuilding[0] ^ findbuilding[1]))
			{
				if (findbuilding[2]) ans = 90;
			}
			else
			{
				int[] und = { 2, 0, 3, 1 };
				int cnt = 0;
				for (int i = 0; (findbuilding[und[i]]) || !(findbuilding[und[(i + 1) % 4]]); ++i) { if (i > 4) Debug.Log(i); ++cnt; }
				if (con_num == 1) ans = 90 * (cnt);
				else if (con_num == 2) ans = 90 * (cnt + 1);
				else if (con_num == 3) ans = 90 * (cnt + 2);
			}
			return new KeyValuePair<Sprite, float>(sprite, ans);
		}

		private void ChooseImage()
		{
			var pair = GetImageAndAngles(this.transform.position);
			spr.sprite = pair.Key;
			transform.eulerAngles = new Vector3(0, 0, pair.Value);
		}
	}
}