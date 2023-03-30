using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase
{
	///<summary>
	///方块类
	///</summary>
	public class Block : MonoBehaviour
	{
		[SerializeField]
		protected int hp = -1;
		public Vector2Int size = new Vector2Int(1, 1);

		public bool Dismantle()
		{
			return false;
		}
	}

	/// <summary>
	/// 建筑基类
	/// </summary>
	public class BaseBuild : Block
	{
		[SerializeField]
		protected int maxInvent = 0;
		protected int localip;
		protected LogistNet fatherLogist = null;
		protected EnergyNet fatherEngrgy;
		protected Inventory invent = null;

		/// <summary>
		/// 获取IP
		/// </summary>
		/// <returns></returns>
		public string GetIP()
		{
			if (fatherLogist == null)
				return localip.ToString();
			else
				return fatherLogist.manager.GetIP() + '.' + localip;
		}
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
		public LogistCentral manager;
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