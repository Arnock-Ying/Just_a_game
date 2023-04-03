using System.Collections;
using UnityEngine;
using GameBase;

namespace Logist
{
	public class InterFace : Block
	{
		public BaseBuild block;
		public LogistPipe pipe;

		public override void DestroyBlock()
		{
			block.InterFaces.Remove(this);
			Destroy(this.gameObject);
		}
	}
}