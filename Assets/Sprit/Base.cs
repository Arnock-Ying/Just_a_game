using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace GameBase
{
	/// <summary>
	/// ��Ʒ��
	/// </summary>
	public class Item
	{
		readonly public static string[] itemName;
		public int id;
		public int count;
		public string name { get { return itemName[id]; } }
	}

	/// <summary>
	/// ��������
	/// </summary>
	public class LogistNet
	{
		public LogistCentral manager;
	}

	/// <summary>
	/// ��������
	/// </summary>
	public class EnergyNet
	{
		public float power;
		public float load;

	}

	/// <summary>
	/// ���
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
	/// ��������
	/// </summary>
	public class BaseBuild : MonoBehaviour
	{
		private int localip;
		private LogistNet fatherLogist = null;
		private EnergyNet fatherEngrgy = null;
		private Inventory invent = null;

		public void Start()
		{
			if (BulidManager.Instend != null)
			{
				BulidManager.BuildList.Add(this);
			}
		}

		/// <summary>
		/// ��ȡIP
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
}