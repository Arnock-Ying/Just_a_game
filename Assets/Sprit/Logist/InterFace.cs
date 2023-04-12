using System.Collections;
using UnityEngine;
using GameBase;

namespace Logist
{
	public class InterFace : Block
	{
		public BaseBuild build;
		public LogistPipe pipe;
		public Dircation dir;
		[SerializeField]
		protected byte localip;
		private Router router = new(null);
		public Router Router { get => router; }
		public byte Ip { get => localip; }
		private LogistNetBlock parentLogist = null;
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

		private void FixedUpdate()
		{
			debug = router.ToString();
			AnswerLogist();
		}


		public void Init()
		{

		}

		public void UpdateIp(ushort[] iptabe)
		{
			router.ChangeRoute(iptabe, (Dircation)((int)dir ^ 1));
		}
		public void SendRouter()
		{
			pipe.setRelayRoute(Router.MakeTable(localip, dir), dir);
		}

		public void AskLogist(Item item, int high = 0)//由建筑主动拉起请求发送
		{
			ParentLogist.ParentNet.AskQueue.Push(localip, item, high);
		}

		public void AnswerLogist()
		{
			//todo-> 对多线程的优化
			
			//todo-> 查找建筑内库存，
			//获取库存对应的网络内请求
			//没有=>return

			//todo-> 物品封装发包
		}


		public override void DestroyBlock()
		{
			if (parentLogist != null)
				if (parentLogist.ParentNet != null)
				{
					parentLogist.ParentNet.DelIp(this.ParentLogist);
					//todo--销毁网络块
				}
			build.InterFaces.Remove(this);
			Destroy(this.gameObject);
		}
	}
}