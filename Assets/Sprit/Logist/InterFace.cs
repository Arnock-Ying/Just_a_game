using System.Collections;
using UnityEngine;
using GameBase;
using System.Collections.Generic;

namespace Logist
{
    public class InterFace : Block
    {
        private BaseBuild build;
        public BaseBuild Build { get => build; set { build = value; invent = build.Invent; } }
        public LogistPipe pipe;
        public Dircation dir;
        [SerializeField]
        protected byte localip;
        private Router router = new(null);
        private Inventory invent = null;
        private KeyValuePair<byte, int>? answer = null;
        private string answerid;
        private List<Item> asks = new();
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
            if (answer != null)
            {
                SendPeakage();
            }
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

        public void AskLogist(Item item, int high = 4)//由建筑主动拉起请求发送
        {
            asks.Add(item);
            ParentLogist.ParentNet.PushAskQueue(localip, item, high);
        }

        public bool AnswerLogist(string id, int maxPackage = 1)
        {
            //对多线程的优化

            //查找建筑内库存，
            //获取库存对应的网络内请求
            //没有=>return
            //有=>物品封装发包
            if (answer != null) return false;
            int have_number = invent.Contains(id);
            if (have_number <= 0) return false;
            if (have_number < maxPackage) maxPackage = have_number;

            answer = parentLogist.ParentNet.AskQueue.Answer(id, maxPackage);
            answerid = id;
            if (answer == null) return false;
            return true;
        }

        private void SendPeakage()
        {
            if (answer == null) return;
            //握手，握手失败则忽视请求
            if (!ParentLogist.ParentNet.GetInterFace(answer.Value.Key).GetReanswer(answerid, answer)) return;
            //物品封装发包
            Item item = build.PopItem(answerid, answer.Value.Value);
            GameObject obj = new GameObject();
            obj.AddComponent<TrafficItems>().Init(item, answer.Value.Key, this);

            answer = null;
            return;
        }

        public virtual bool GetReanswer(string id, KeyValuePair<byte, int>? answer)
        {
            //接受响应并通知建筑
            if (answer == null) return false;
            if (answer.Value.Key != localip) return false;

            for (int i = 0; i < asks.Count; ++i)
            {
                if (asks[i].id == id)
                {
                    if (asks[i].count > answer.Value.Value)
                    {
                        asks[i].count -= answer.Value.Value;
                        return true;
                    }
                    else if (asks[i].count == answer.Value.Value)
                    {
                        asks.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public override void DestroyBlock()
        {
            if (parentLogist != null)
                if (parentLogist.ParentNet != null)
                {
                    parentLogist.ParentNet.DelIp(this.ParentLogist);
                }
            build.InterFaces.Remove(this);
            Destroy(this.gameObject);
        }
    }
}