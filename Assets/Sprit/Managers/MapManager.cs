using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameBase;

namespace Manager
{
	public class MapManager : MonoBehaviour
	{
		public Dictionary<Vector2Int, BlockControl> blockList = new();

		private static MapManager instend = null;
		public static MapManager Instend { get { return instend; } }
		public static Dictionary<Vector2Int, BlockControl> Blocks { get { return instend.blockList; } }
		public static Block GetBlock(int x, int y)
		{ 
			if (instend.blockList.ContainsKey(new Vector2Int(Mathf.FloorToInt(x / 256.0f), Mathf.FloorToInt(y / 256.0f))))
				return instend.blockList[new Vector2Int(Mathf.FloorToInt(x / 256.0f), Mathf.FloorToInt(y / 256.0f))].GetBlock(x % 256, y % 256);
			else return null;
			//Debug.Log("get:" + new Vector2Int(Mathf.FloorToInt(x / 256.0f), Mathf.FloorToInt(y / 256.0f)) + new Vector2Int(x % 256, y % 256) + (b ? b : " null"));
		}
		public static Block GetBlock(Vector2Int v)
		{
			return GetBlock(v.x, v.y);
		}
		public static void SetBlock(int x, int y, Block block)
		{
			//Debug.Log("set:" + new Vector2Int(Mathf.FloorToInt(x / 256.0f), Mathf.FloorToInt(y / 256.0f)) + new Vector2Int(x % 256, y % 256) + block);
			if (!instend.blockList.ContainsKey(new Vector2Int(Mathf.FloorToInt(x / 256.0f), Mathf.FloorToInt(y / 256.0f))))
				instend.blockList[new Vector2Int(Mathf.FloorToInt(x / 256.0f), Mathf.FloorToInt(y / 256.0f))] = new();
			instend.blockList[new Vector2Int(Mathf.FloorToInt(x / 256.0f), Mathf.FloorToInt(y / 256.0f))].SetBlock(x % 256, y % 256, block);
		}
		public static void SetBlock(Vector2Int v, Block block)
		{
			SetBlock(v.x, v.y, block);
		}

		public static void SetBuild(Vector2 pos, Vector2Int size, Block block)
		{
			Vector2Int posInt = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
			for (int i = posInt.x - size.x / 2; i < posInt.x + Mathf.CeilToInt(size.x / 2.0f); ++i)
				for (int j = posInt.y - size.y / 2; j < posInt.y + Mathf.CeilToInt(size.y / 2.0f); ++j)
					SetBlock(i, j, block);

		}
		public static bool GetBuild(Vector2 pos, Vector2Int size)
		{
			Vector2Int posInt = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
			for (int i = posInt.x - size.x / 2; i < posInt.x + Mathf.CeilToInt(size.x / 2.0f); ++i)
				for (int j = posInt.y - size.y / 2; j < posInt.y + Mathf.CeilToInt(size.y / 2.0f); ++j)
					if (GetBlock(i, j))
						return true;
			return false;
		}
		public void Awake()
		{
			if (instend == null)
				instend = this;
			else
				Destroy(gameObject);
		}
	}
}