using System.Collections;
using UnityEngine;
using GameBase;

namespace Logist
{
	public class InterFace : Block
	{
		public BaseBuild build;
		public LogistPipe pipe;
		[SerializeField]
		protected byte localip;
		public byte Ip { get => localip; }
		protected LogistNetBlock parentLogist = null;
		public override LogistNetBlock ParentLogist { get => parentLogist; set => parentLogist = value; }

		/// <summary>
		/// 获取IP
		/// </summary>
		/// <returns></returns>
		public string GetIP()
		{
			if (parentLogist == null)
				return localip.ToString();
			else
				return $"{parentLogist.ParentNet.Manager.GetIP()}.{localip}";
		}

		/// <summary>
		/// 设置ip 为物流网络分配ip设置的接口，请勿调用！
		/// </summary>
		/// <returns></returns>
		public void SetIP(byte i)
		{
			localip = i;
		}


		public void Init()
		{

		}

		public override void DestroyBlock()
		{
			if (parentLogist != null)
				if (parentLogist.ParentNet != null)
					parentLogist.ParentNet.DelIp(this);
			build.InterFaces.Remove(this);
			Destroy(this.gameObject);
		}
	}
}