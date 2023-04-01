using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameBase;
using Manager;

namespace Logist
{
	public class LogistCentral : BaseBuild
	{
		private LogistNet managedlogist;
		private SpriteRenderer spriteRenderer;

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