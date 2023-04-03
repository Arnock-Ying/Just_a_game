using System.Collections;
using UnityEngine;
using GameBase;

namespace Logist
{
	public class InterFace : Block
	{
		public BaseBuild block;
		public LogistPipe pipe;

		public void Remove()
		{
			block.InterFaces.Remove(this);
		}
		  
	}
}