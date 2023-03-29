using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

namespace Manager
{
	public class BulidManager : MonoBehaviour
	{
		public List<BaseBuild> baseBuilds = new();

		private static BulidManager instend = null;
		public static BulidManager Instend { get { return instend; } }
		public static List<BaseBuild> BuildList { get { return instend.baseBuilds; } }
		public void Awake()
		{
			if (instend == null)
				instend = this;
			else
				Destroy(gameObject);
		}

	}
}