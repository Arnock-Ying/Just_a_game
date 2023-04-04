using System.Collections;
using UnityEngine;
using GameBase;

namespace Logist
{
	public class InterFace : Block
	{
		public BaseBuild build;
		public LogistPipe pipe;

		public void Init()
		{
			
		}

		public override void DestroyBlock()
		{
			build.InterFaces.Remove(this);
			Destroy(this.gameObject);
		}
	}
}