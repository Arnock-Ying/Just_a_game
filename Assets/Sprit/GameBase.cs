using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Logist;

namespace GameBase
{
	///<summary>
	///������
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
	/// ��������
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

	///<summary>
	///�����ܵ�
	/// </summary>
	//public class LogistPipe : Block
	//{

	//}

	///<summary>
	///�����ܵ�
	///</summary>
	public class EnergyPipe : Block
	{

	}

	///<summary>
	///����
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
	/// ��Ʒ��
	/// </summary>
	public class Item
	{
		public string id;
		public int count;
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