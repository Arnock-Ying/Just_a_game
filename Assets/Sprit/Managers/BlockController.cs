using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameBase;

namespace Manager
{
	public class BlockControl
	{
		private Block[,] blocks = new Block[256, 256];
		public BlockControl()
		{
			for (int i = 0; i < 256; ++i)
				for (int j = 0; j < 256; ++j)
					blocks[i, j] = null;
		}
		public Block GetBlock(int x, int y)
		{
			return blocks[x, y];
		}

		public void SetBlock(int x, int y, Block block)
		{
			blocks[x, y] = block;
		}
	}
}