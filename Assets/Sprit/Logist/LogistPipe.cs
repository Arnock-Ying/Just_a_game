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
		LogistNetBlock net;

		Router router = null;
		ushort[][] temp_rout = new ushort[4][], inter_rout = new ushort[4][];

		int con_num = 0;    //连接数量
		Block[] findbuilding = new Block[4];//上下左右是否有建筑

		private LogistNetBlock parentLogist = null;
		public override LogistNetBlock ParentLogist { get => parentLogist; set => parentLogist = value; }

		private void Start()
		{
			BuildPipe(true);
		}

		private void FixedUpdate()
		{
			for (int i = 0; i < 4; ++i)
			{
				if (temp_rout[i] != null) relayRoute(temp_rout[i], (Dircation)i);
				temp_rout[i] = inter_rout[i];
				inter_rout[i] = null;
			}
			debug = (parentLogist == null ? "nullnet" :
				  $" LogistNetBlock = {(parentLogist.Inter == null ? "null" : parentLogist.Inter.Build.name)} : {parentLogist.id}")
				 + $" LogistNet = {(parentLogist.ParentNet == null ? "null" : "Net id:" + parentLogist.ParentNet.id)}\n";

			if (router != null)
			{
				debug += $"router {parentLogist.ParentNet.MaxIpNum}:\n";
				for (byte i = 0; i < parentLogist.ParentNet.MaxIpNum; ++i)
					debug += $"ip: {i}, dir: {router.Dir(i)}, len: {router.Len(i)}\n";
			}
		}

		public void setRelayRoute(ushort[] rout, Dircation dir)
		{
			inter_rout[(int)dir] = rout;
		}
		private void relayRoute(ushort[] rout, Dircation dir)
		{
			if (con_num <= 1) return;
			if (router == null)
			{
				for (int i = 0; i < 256; ++i)
					if (rout[i] != 0) rout[i] += 4;
				for (int i = 0; i < 4; ++i)
					if ((int)dir != (i ^ 1) && findbuilding[i] != null)
					{
						if (findbuilding[i] is LogistPipe pipe)
						{
							Debug.Log(transform.position + "pipe with null nouter to" + (Dircation)i);
							pipe.setRelayRoute(rout, (Dircation)(i));
						}
						else if (findbuilding[i] is InterFace inter) inter.UpdateIp(rout);
					}
			}
			else
			{
				var back = router.ChangeRoute(rout, dir);
				for (int i = 0; i < 4; ++i)
					if (back.Key && ((int)dir != (i ^ 1)) && findbuilding[i] != null)
					{
						if (findbuilding[i] is LogistPipe pipe) pipe.setRelayRoute(router.CopyIpTable(), (Dircation)(i));
						else if (findbuilding[i] is InterFace inter) inter.UpdateIp(router.CopyIpTable());
					}
					else if (back.Value && findbuilding[i] != null)
					{
						if (findbuilding[i] is LogistPipe pipe) pipe.setRelayRoute(router.CopyIpTable(), (Dircation)(i));
						else if (findbuilding[i] is InterFace inter) inter.UpdateIp(router.CopyIpTable());
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
				int[] step = { 0, 0, -1, 1, 1, -1, 0, 0 };
				var block = MapManager.GetBlock(pos.x + step[i], pos.y + step[i + 4]);
				//Debug.Log(block);

				if (block != null)
				{
					if (block is BaseBuild build)
					{
						con_num++;
						if (findbuilding[i] is not InterFace nowinter || nowinter.Build != build)
						{
							InterFace inter = Instantiate(Resources.Load<GameObject>("Pipe/InterFace")).GetComponent<InterFace>();
							inter.gameObject.transform.position = new Vector3(transform.position.x + step[i] * 0.5f, transform.position.y + step[i + 4] * 0.5f, -1.5f);
							build.InterFaces.Add(inter);
							inter.Build = build;
							inter.pipe = this;
							inter.dir = (Dircation)(i ^ 1);
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

			if (rebuild)
			{
				//if (parentLogist == null) parentLogist = new LogistNetBlcok();
				//处理物流网络信息
				UpdateParentLogist();
			}
		}

		///<summary>
		///更新物流网络信息
		///</summary>
		public void UpdateParentLogist()
		{
			//遍历四个方向
			int[] counts = new int[4];
			for (int i = 0; i < 4; ++i)
			{
				if (findbuilding[i] == null) counts[i] = -1;
				else if (findbuilding[i] is LogistPipe pipe)
					counts[i] = pipe.ParentLogist.Count;
				else if (findbuilding[i] is InterFace inter)
					counts[i] = 0;
			}

			//寻找较小的网络块
			int min = 0;
			int minnum = -1;
			for (int i = 0; i < 4; ++i)
			{
				if (minnum == -1 || (minnum > counts[i] && counts[i] >= 0))
				{
					min = i;
					minnum = counts[i];
				}
			}

			//接入，如果四周没有则创建
			if (minnum == -1) this.parentLogist = new();
			else if (findbuilding[min] is InterFace inter)
			{
				this.parentLogist = new();
				parentLogist.Inter = inter;
				if (inter.Build is LogistCentral)
				{
					parentLogist.ParentNet.SetManager(inter);
				}
			}
			else
			{
				parentLogist = findbuilding[min].ParentLogist;
			}
			parentLogist.Add(this);

			//合并空网络块
			if (parentLogist.Inter == null)
			{
				//查找较小的空网络块
				for (int i = 0; i < 4; ++i)
				{
					if (findbuilding[i] == null) counts[i] = -2;
					else if (findbuilding[i] is LogistPipe pipe)
						counts[i] = (pipe.ParentLogist.Inter == null ? -1 : pipe.ParentLogist.Count);
					else if (findbuilding[i] is InterFace)
						counts[i] = 0;
				}
				int min2 = 0;
				int minnum2 = -2;
				for (int i = 0; i < 4; ++i)
				{
					if (minnum2 < 0 || (minnum2 > counts[i] && counts[i] >= 0))
					{
						min2 = i;
						minnum2 = counts[i];
					}
				}
				//合并
				if (minnum2 != -2)
				{
					findbuilding[min2].ParentLogist.Marge(this.ParentLogist);
				}
			}


			//查找周围的网络块，合并网络块和物流网络
			for (int i = 0; i < 4; ++i)
			{
				if (findbuilding[i] is LogistPipe pipe)
				{
					if (pipe.ParentLogist.Inter == null)
						ParentLogist.Marge(pipe.ParentLogist);
					else
					//合并两非空物流网络
					//周围网络非空，非相同物流网络，合并后网络不会过大，网络管理器不会出现冲突
					//其实应该写成LogistNet的静态，但是QAQ
					if ((ParentLogist.ParentNet != pipe.ParentLogist.ParentNet)
						&& LogistNet.BuildSum(ParentLogist.ParentNet, pipe.ParentLogist.ParentNet)
							<= Math.Max(ParentLogist.ParentNet.MaxIpNum, pipe.ParentLogist.ParentNet.MaxIpNum)
						&& !((ParentLogist.ParentNet.Manager is not null && ParentLogist.ParentNet.Manager.Build is LogistCentral)
							&& (pipe.ParentLogist.ParentNet.Manager is not null && pipe.ParentLogist.ParentNet.Manager.Build is LogistCentral)))

					{
						if (ParentLogist.ParentNet.MaxIpNum > pipe.ParentLogist.ParentNet.MaxIpNum)
							ParentLogist.ParentNet.Marge(pipe.ParentLogist.ParentNet);
						else
							pipe.ParentLogist.ParentNet.Marge(ParentLogist.ParentNet);
					}
				}
			}


			//释放合并所产生的垃圾空间
			GC.Collect();
			return;
		}

		public static KeyValuePair<Sprite, float> GetImageAndAngles(Vector3 position)
		{
			Vector2Int pos = new(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
			//Debug.Log(pos);
			int con_num = 0;
			bool[] findbuilding = new bool[4];
			int[] step = { -1, 1, 0, 0, 0, 0, 1, -1 };
			for (int i = 0; i < 4; i++)
			{
				findbuilding[i] = false;


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