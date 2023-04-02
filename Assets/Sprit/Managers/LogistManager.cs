using System.Collections;
using UnityEngine;

namespace Manager
{
	public class LogistManager : MonoBehaviour
	{

		private static LogistManager instend = null;
		public static LogistManager Instend { get { return instend; } }

		[SerializeField]
		private Sprite[] pipeImage;
		public Sprite[] PipeImage { get { return pipeImage; } }
		public void Awake()
		{
			if (instend == null)
				instend = this;
			else
				Destroy(gameObject);
		}
	}
}