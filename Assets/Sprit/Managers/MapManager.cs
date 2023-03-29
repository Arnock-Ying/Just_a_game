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
			return instend.blockList[new Vector2Int(Mathf.FloorToInt(x / 256), Mathf.FloorToInt(y / 256))].GetBlock(x % 256, y % 256);
		}

		public static void GetBlock(int x, int y, Block block)
		{
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