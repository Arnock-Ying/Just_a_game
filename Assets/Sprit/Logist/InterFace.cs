using System.Collections;
using UnityEngine;
using GameBase;
using System.Collections.Generic;

namespace Logist
{
    public class InterFace : Block
    {
        public BaseBuilding building;
        public LogistPipe pipe;
        public Dircation dir;
        [SerializeField]
        protected byte localip;
        private Router router = new(null);
        private (byte, int)? answer = null;
        private string answerid;
        public List<Item> asks = null;
        public Router Router { get => router; }
        public byte Ip { get => localip; }
        public override LogistNetBlock ParentLogist { get; set; } = null;

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public string GetIP()
        {
            if (ParentLogist == null)
                return localip.ToString();
            else
                return $"{ParentLogist.ParentNet.Manager.GetIP()}.{localip}";
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
                int i = (SendPeakage());

                if (answer != null && i < answer.Value.Item2)
                    ParentLogist.ParentNet.PushAskQueue(answer.Value.Item1, answerid, answer.Value.Item2 - i);
            }
        }

        public void UpdateIp(ushort[] iptabe)
        {
            router.ChangeRoute(iptabe, (Dircation)((int)dir ^ 1));
            router.Set(localip, 0, 0);
        }
        public void SendRouter()
        {
            Dircation _dir = (Dircation)((int)dir ^ 1);
            pipe.setRelayRoute(Router.MakeTable(localip, dir), dir);
            pipe.setRelayRouteCommand(LogistCommand.Update, dir);
        }

        public void GetCommand(LogistCommand command)
        {
            Debug.Log(transform.position + " input " + command);
            switch (command)
            {
                case LogistCommand.Nulldate:
                    break;
                case LogistCommand.Update:
                    break;
                case LogistCommand.Newdate:
                    SendRouter();
                    break;
                case LogistCommand.Outdate:
                    break;
            }
        }
        public void AskLogist(Item item, int high = 4)//由建筑主动拉起请求发送
        {
            asks.Add(item);
            ParentLogist.ParentNet.UpdataAskQueue(localip, item, high);
        }

        public bool AnswerLogist(string id, int maxPackage = 1)
        {
            //对多线程的优化


            //查找建筑内库存，
            //获取库存对应的网络内请求
            //没有=>return
            //有=>物品封装发包
            if (answer != null) return false;

            lock (building)
            {
                int have_number = building.Contains(id);
                if (have_number <= 0) return false;
                if (have_number < maxPackage) maxPackage = have_number;

                answer = ParentLogist.ParentNet.AskQueue.Answer(id, maxPackage);
                if (answer == null) return false;
                answerid = id;

                return true;
            }
        }

        private int SendPeakage()
        {
            if (answer == null) return 0;
            //握手，握手失败则忽视请求
            var des = ParentLogist.ParentNet.GetInterFace(answer.Value.Item1);
            if (des == null || !des.GetReanswer(answerid, answer))
            {
                Debug.Log(ParentLogist.ParentNet.ToString());
                Debug.LogError($"to {answer.Value.Item1} shake hands error with des {(des == null ? "null" : des.name)}!");
                answer = null;
                return 0;
            }
            //物品封装发包

            Item item = building.PopItem(answerid, answer.Value.Item2);
            if (item == null) return 0;
            GameObject obj = new GameObject($"{item.id}-{item.count} peakage");
            obj.AddComponent<TrafficItems>().Init(item, answer.Value.Item1, this);

            answer = null;
            return item.count;
        }

        public virtual bool GetReanswer(string id, (byte, int)? answer)
        {
            //接受响应并通知建筑
            if (answer == null) return false;
            if (answer.Value.Item1 != localip) return false;

            for (int i = 0; i < asks.Count; ++i)
            {
                if (asks[i].id == id)
                {
                    if (asks[i].count > answer.Value.Item2)
                    {
                        asks[i].count -= answer.Value.Item2;
                        return true;
                    }
                    else if (asks[i].count == answer.Value.Item2)
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
            if (ParentLogist != null)
                if (ParentLogist.ParentNet != null)
                {
                    //析构有大问题
                    //ParentLogist.ParentNet.DelIp(this.ParentLogist);
                    Debug.Log($"del {localip}");
                }
            building.InterFaces.Remove(this);
            Destroy(this.gameObject);
        }

        public void InputItem(Item item)
        {
            if (!building.Invent.Input(item.id, item.count))
                Debug.Log($"Input {item.id} {item.count} Error");
        }
    }
}