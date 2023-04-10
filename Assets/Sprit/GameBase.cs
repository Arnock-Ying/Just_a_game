using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Logist;
using Manager;

namespace GameBase
{
	///<summary>
	///方块类
	///</summary>
	public class Block : MonoBehaviour
	{
		[SerializeField]
		protected int hp = -1;
		public Vector2Int size = new(1, 1);
		public virtual LogistNetBlock ParentLogist { get { return null; } set { throw new System.Exception("have no LogistNet!"); } }
		//debug
		[SerializeField]
		[TextArea]
		protected string debug;
		public bool Dismantle()
		{
			return false;
		}

		protected void UpdateLoglist()
		{
			Vector2Int pos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
			for (int i = pos.x - Mathf.FloorToInt(size.x / 2.0f); i < pos.x + Mathf.CeilToInt(size.x / 2.0f); ++i)
			{
				if (MapManager.GetBlock(i, pos.y - Mathf.FloorToInt(size.y / 2.0f) - 1) is LogistPipe pipe1)
				{
					pipe1.BuildPipe(false);
				}
				if (MapManager.GetBlock(i, pos.y + Mathf.CeilToInt(size.y / 2.0f)) is LogistPipe pipe2)
				{
					pipe2.BuildPipe(false);
				}
			}
			for (int i = pos.y - Mathf.FloorToInt(size.y / 2.0f); i < pos.y + Mathf.CeilToInt(size.y / 2.0f); ++i)
			{
				if (MapManager.GetBlock(pos.x - Mathf.FloorToInt(size.x / 2.0f) - 1, i) is LogistPipe pipe1)
				{
					pipe1.BuildPipe(false);
				}
				if (MapManager.GetBlock(pos.x + Mathf.CeilToInt(size.x / 2.0f), i) is LogistPipe pipe2)
				{
					pipe2.BuildPipe(false);
				}
			}
		}

		public virtual void DestroyBlock()
		{
			MapManager.SetBuild(transform.position, size, null);
			UpdateLoglist();
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// 建筑基类
	/// </summary>
	public class BaseBuild : Block
	{
		[SerializeField]
		protected int maxInvent = 0;
		[SerializeField]
		protected byte localip;
		protected LogistNetBlock privateLogist = null;
		protected EnergyNet privateEngrgy;
		protected Inventory invent = null;
		public byte Ip { get => localip; }
		public override LogistNetBlock ParentLogist { get => privateLogist; set => privateLogist = value; }
		public List<InterFace> InterFaces { get; } = new();

		/// <summary>
		/// 获取IP
		/// </summary>
		/// <returns></returns>
		public string GetIP()
		{
			if (privateLogist == null)
				return localip.ToString();
			else
				return $"{privateLogist.ParentNet.manager.GetIP()}.{localip}";
		}

		/// <summary>
		/// 设置ip 为物流网络分配ip设置的接口，请勿调用！
		/// </summary>
		/// <returns></returns>
		public void SetIP(byte i)
		{
			localip = i;
		}

		public override void DestroyBlock()
		{
			privateLogist.ParentNet.DelIp(this);
			base.DestroyBlock();
		}
	}

	public class Formula
	{
		List<Item> material = new();
		public List<Item> Material { get { return material; } }
		List<Item> product = new();
		public List<Item> Product { get { return product; } }
		public float production_time = 1;//生产需要的时间
		public bool isenable = false;   //是否启用该配方
		Formula() { }
		Formula(List<Item> mat, List<Item> pro = null)
		{
			material = mat;
			product = pro;
		}
	}

	public class ProductionBuilding : BaseBuild
	{
		protected Formula formula;
		protected float efficiency = 1;
	}

	///<summary>
	///物流管道
	/// </summary>
	//public class LogistPipe : Block
	//{

	//}

	///<summary>
	///能量管道
	///</summary>
	public class EnergyPipe : Block
	{

	}

	///<summary>
	///生物
	///</summary>
	public class Biont : MonoBehaviour
	{
		protected int hp, atk;
		protected float speed;

		public virtual bool onDeath()
		{
			return false;
		}
	}

	/// <summary>
	/// 物品类
	/// </summary>
	public class Item
	{
		public string id;
		public int count;
	}

	/// <summary>
	/// 物流网络
	/// </summary>
	public class LogistNet
	{
		public BaseBuild manager { get { return builds[0]; } }
		private int maxIpNum = 8;
		private int count = 1;
		static private readonly int selfNetNum = 8;
		public readonly int id;
		static int nowid = 0;
		public List<LogistNetBlock> Blocks { get; } = new();
		public int Count { get => count; }
		public int MaxIpNum { get => maxIpNum; }

		private readonly BaseBuild[] builds = new BaseBuild[256];

		public LogistNet()
		{
			id = nowid;
			nowid++;
		}
		public bool SetManager(BaseBuild manager)
		{
			if (manager == null)
			{
				maxIpNum = 0;
				return false;
			}
			if (manager is LogistCentral logist)
			{
				maxIpNum = logist.MaxIPNum();
				if (maxIpNum > 256) maxIpNum = 256;
			}
			else
			{
				maxIpNum = selfNetNum;
			}

			builds[0] = manager;
			manager.SetIP(0);
			manager.ParentLogist.ParentNet = this;
			return true;
		}
		public bool SetIp(BaseBuild block)
		{
			int min = -1;
			for (int i = 1; i < maxIpNum; ++i)
			{
				if (builds[i] == null && min == -1)
					min = i;
				if (block == builds[i])
					return true;
			}

			if (min == -1) return false;

			builds[min] = block;
			block.SetIP((byte)min);
			block.ParentLogist.ParentNet = this;
			count += 1;
			return true;
		}
		public bool DelIp(BaseBuild block)
		{
			int ip = block.Ip;
			if (builds[ip] == block)
			{
				builds[ip] = null;
				block.ParentLogist = null;
				--count;
				return true;
			}
			return false;
		}

		public void Marge(LogistNet net)
		{
			if (net == this) return;
			foreach (var i in net.Blocks)
			{
				if (!Blocks.Contains(i))
				{
					Blocks.Add(i);
					i.ParentNet = this;
				}
			}
			net.Blocks.Clear();
		}
		public static int BuildSum(LogistNet net1, LogistNet net2)
		{
			int sum = net1.count + net2.count - 1;
			for (int i = 1; i < net1.maxIpNum; ++i)
			{
				for (int j = i; j < net2.maxIpNum; ++j)
				{
					if (net1.builds[i] == net2.builds[j]) --sum;
				}
			}
			return sum;
		}
	}

	public class LogistNetBlock
	{
		public readonly int id;
		static int nowid = 0;
		readonly List<LogistPipe> pipes = new();
		public LogistNet ParentNet { get; set; } = new();
		public int Count { get => pipes.Count; }
		public BaseBuild Build { get; set; } = null;
		public LogistNetBlock()
		{
			id = nowid;
			nowid++;
			ParentNet.Blocks.Add(this);
		}
		public void Add(LogistPipe pipe)
		{
			pipes.Add(pipe);
		}

		public void Remove(LogistPipe pipe)
		{
			pipes.Remove(pipe);
		}

		public void Marge(LogistNetBlock netblock)
		{
			if (netblock.pipes.Count == 0) return;
			if (netblock == this) return;
			foreach (var i in netblock.pipes)
			{
				pipes.Add(i);
				i.ParentLogist = this;
			}
			netblock.pipes.Clear();
		}
	}

	/// <summary>
	/// 能量网络
	/// </summary>
	public class EnergyNet
	{
		public float power;
		public float load;

	}

	/// <summary>
	/// 库存
	/// </summary>
	public class Inventory
	{
		private int maxCount = 0;
		private int count = 0;

		private Dictionary<string, int> items;

		public int Count { get { if (maxCount == 0) return 0; else return count; } }
		public int MaxCount { get { return maxCount; } }
		public Inventory()
		{
			maxCount = 0;
		}
		public Inventory(int size)
		{
			maxCount = size;
			items = new();
		}
		public bool Input(string id, int number)
		{
			if (maxCount == 0) return false;
			if (count + number > maxCount)
				return false;
			if (items.ContainsKey(id))
			{
				items[id] += number;
				count += number;
			}
			else
			{
				items[id] = number;
				count += number;
			}
			return true;
		}
		public bool Output(string id, int number)
		{
			if (maxCount == 0) return false;
			if (items.ContainsKey(id))
			{
				if (items[id] > number)
				{
					items[id] -= number;
					count -= number;
				}
				else if (items[id] == number)
				{
					if (items.Remove(id))
						count -= number;
				}
				else
					return false;
			}
			else
				return false;
			return true;
		}

		public int Get(string id)
		{
			return items[id];
		}

		public string GetLog()
		{
			if (items == null) return "null";
			string s = "{";
			foreach (var i in items)
			{
				s += i.Key + " : " + i.Value + " , ";
			}
			s += "}";
			return s;
		}
	}

}