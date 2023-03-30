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
			if (instend.blockList.ContainsKey(new Vector2Int(Mathf.FloorToInt(x / 256), Mathf.FloorToInt(y / 256))))
				return instend.blockList[new Vector2Int(Mathf.FloorToInt(x / 256), Mathf.FloorToInt(y / 256))].GetBlock(x % 256, y % 256);
			else return null;
		}

		public static void SetBlock(int x, int y, Block block)
		{
			if (!instend.blockList.ContainsKey(new Vector2Int(Mathf.FloorToInt(x / 256), Mathf.FloorToInt(y / 256))))
				instend.blockList[new Vector2Int(Mathf.FloorToInt(x / 256), Mathf.FloorToInt(y / 256))] = new();
			instend.blockList[new Vector2Int(Mathf.FloorToInt(x / 256), Mathf.FloorToInt(y / 256))].SetBlock(x % 256, y % 256, block);
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