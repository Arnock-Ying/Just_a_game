using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase
{
	/// <summary>
	/// 物品类
	/// </summary>
	public class Item
	{
		readonly public static string[] itemName;
		public int id;
		public int count;
		public string name { get { return itemName[id]; } }
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
		private int maxCount;
		private int count;

		private Item[] items;

		public int Count { get { return count; } }
		public int MaxCount { get { return maxCount; } }
	}

	/// <summary>
	/// 建筑基类
	/// </summary>
	public class BaseBuild : MonoBehaviour
	{
		private int localip;
		private LogistNet fatherLogist = null;
		private EnergyNet fatherEngrgy;
		private Inventory invent = null;

		public string GetIP()
		{
			if (fatherLogist == null)
				return localip.ToString();
			else
				return fatherLogist.manager.GetIP() + '.' + localip;
		}
	}
}