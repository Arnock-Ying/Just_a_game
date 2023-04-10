using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameBase;
using Manager;

namespace Logist
{
	public class LogistCentral : BaseBuild
	{
		private LogistNetBlock managedlogist;
		private SpriteRenderer spriteRenderer;
		[SerializeField]
		private int maxIpNum = 32;
		public int MaxIPNum() { return maxIpNum; }

		void Start()
		{
			MapManager.SetBuild(transform.position, size, this);
			var col = gameObject.GetComponent<BoxCollider>();
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		void Update()
		{

		}

	}
}