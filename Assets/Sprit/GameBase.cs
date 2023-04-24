using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

using Logist;
using Manager;

namespace GameBase
{

    ///<summary>
    ///方块类
    ///</summary>
    public class Block : MonoBehaviour
    {
        [SerializeField]
        protected int hp = -1;
        public Vector2Int size = new(1, 1);
        public virtual LogistNetBlock ParentLogist { get { return null; } set { throw new System.Exception("have no LogistNet!"); } }
        //debug
        [SerializeField]
        [TextArea(1, 100)]
        protected string debug;
        public bool Dismantle()
        {
            return false;
        }

        protected void UpdateLoglist()
        {
            Vector2Int pos = new(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            for (int i = pos.x - Mathf.FloorToInt(size.x / 2.0f); i < pos.x + Mathf.CeilToInt(size.x / 2.0f); ++i)
            {
                if (MapManager.GetBlock(i, pos.y - Mathf.FloorToInt(size.y / 2.0f) - 1) is LogistPipe pipe1)
                {
                    pipe1.BuildPipe(false, true);
                }
                if (MapManager.GetBlock(i, pos.y + Mathf.CeilToInt(size.y / 2.0f)) is LogistPipe pipe2)
                {
                    pipe2.BuildPipe(false, true);
                }
            }
            for (int i = pos.y - Mathf.FloorToInt(size.y / 2.0f); i < pos.y + Mathf.CeilToInt(size.y / 2.0f); ++i)
            {
                if (MapManager.GetBlock(pos.x - Mathf.FloorToInt(size.x / 2.0f) - 1, i) is LogistPipe pipe1)
                {
                    pipe1.BuildPipe(false, true);
                }
                if (MapManager.GetBlock(pos.x + Mathf.CeilToInt(size.x / 2.0f), i) is LogistPipe pipe2)
                {
                    pipe2.BuildPipe(false, true);
                }
            }
        }

        public virtual void DestroyBlock()
        {
            MapManager.SetBuild(transform.position, size, null);
            UpdateLoglist();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 建筑基类
    /// </summary>
    public class BaseBuilding : Block
    {
        [SerializeField]
        protected int maxInvent = 0;

        protected LogistNetBlock privateLogist = null;
        protected EnergyNet privateEngrgy;
        protected Inventory invent;
        private List<Item> asks = new();

        public Inventory Invent { get => invent; }
        public override LogistNetBlock ParentLogist { get => privateLogist; set => privateLogist = value; }
        public List<InterFace> InterFaces { get; } = new();
        public virtual int Contains(string id)
        {
            return invent.Contains(id);
        }

        public virtual Item PopItem(string id, int count)
        {
            return invent.Output(id, count);
        }

        public void AddInterFace(InterFace inter)
        {
            this.InterFaces.Add(inter);
            inter.building = this;
            inter.asks = asks;
        }
    }

    public class Formula
    {
        List<Item> material = new();
        public List<Item> Material { get { return material; } }
        List<Item> product = new();
        public List<Item> Product { get { return product; } }
        public float production_time = 1;//生产需要的时间
        public bool isenable = false;   //是否启用该配方
        public Formula() { }
        public Formula(List<Item> mat, List<Item> pro = null)
        {
            material = mat;
            product = pro;
        }
    }

    public class ProductionBuilding : BaseBuilding
    {
        protected Formula formula = null;
        protected float efficiency = 1;
        protected bool enable = true;
        protected float timer = 0;
        protected const float delayTime = 1;

        bool first = true;
        protected void Start()
        {
            maxInvent = 100;
            invent = new(maxInvent);
            Item wood = new("wood", 1);//临时用用
            List<Item> temp = new();
            temp.Add(wood);
            formula = new(temp);
        }

        protected void Update()
        {
            if (first)
                if (enable)
                {
                    timer += Time.deltaTime;
                    if (timer >= delayTime)
                    {
                        foreach (Item it in formula.Material)
                        {
                            if (invent.Contains(it.id) < maxInvent)
                            {
                                //传进接口里
                                foreach (InterFace iface in InterFaces)
                                {
                                    if (maxInvent - invent.Contains(it.id) > 0)
                                        iface.AskLogist(new Item(it.id, maxInvent - invent.Contains(it.id)));
                                    //first = false;
                                }
                                debug = "请求: " + it.id + " " + maxInvent + "\n内存物品数量: \n" + invent.GetLog();
                            }
                        }
                        timer = 0;
                    }
                }
        }
    }

    ///<summary>
    ///能量管道
    ///</summary>
    public class EnergyPipe : Block
    {

    }

    ///<summary>
    ///生物
    ///</summary>
    public class Biont : MonoBehaviour
    {
        protected int hp, atk;
        protected float speed;

        public virtual bool onDeath()
        {
            return false;
        }
    }

    /// <summary>
    /// 物品类
    /// </summary>
    public class Item
    {
        public string id;
        public int count;

        public Item() { }
        public Item(string i, int c)
        {
            id = i; count = c;
        }

    }

    /// <summary>
    /// 请求队列
    /// </summary>
    public class AskQueue
    {
        static readonly int maxPriority = 10;
        static readonly int maxip = 256;
        Dictionary<string, (int, int)?[]> queues = new();
        byte polling = 0;

        public string[] Keys
        {
            get
            {
                int n = queues.Keys.Count;
                string[] keys = new string[n + 1];
                {
                    int i = 0;
                    foreach (var s in queues.Keys)
                    {
                        keys[i] = s;
                        ++i;
                    }
                }
                return keys;
            }
        }

        public (byte, int)? Tops(string id)
        {
            return queues[id][polling] == null ? null : new(polling, queues[id][polling].Value.Item1);
        }

        public bool UpdataAsk(int ip, Item item, int high = 0)
        {
            return UpdataAsk(ip, item.id, item.count, high);
        }
        private bool UpdataAsk(int ip, string id, int num, int high = 0)
        {
            Debug.Log($"ip:{ip},Item:{id},{num},high:{high}");
            if (ip >= 256 || ip < 0) return false;
            if (num < 0) return false;
            if (high >= maxPriority || high < 0) return false;

            if (!queues.ContainsKey(id))
            {
                queues.Add(id, new (int, int)?[maxip]);
                for (int i = 0; i < maxip; ++i) queues[id][i] = null;
            }

            queues[id][ip] = new(num, high);
            //if (!queues.ContainsKey(id))
            //{
            //    queues.Add(id, new (byte, int)?[maxip]);
            //    Tops.Add(id, null);
            //    for (int i = 0; i < maxPriority; ++i)
            //    {
            //        queues[id][i] = new();
            //    }
            //}
            //queues[id][high].Enqueue(new((byte)ip, num));
            //UpdateTop(id);
            Debug.Log($"Item:ip:{ip},Item:{id},{num},high:{high};\nqueues top of {id} : {(Tops(id) == null ? null : Tops(id).Value)}");
            return true;
        }

        public bool PushAsk(int ip, string id, int num, int high = 0)
        {
            Debug.Log($"ip:{ip},Item:{id},{num},high:{high}");
            if (ip >= 256 || ip < 0) return false;
            if (num < 0) return false;
            if (high >= maxPriority || high < 0) return false;

            if (!queues.ContainsKey(id))
            {
                queues.Add(id, new (int, int)?[maxip]);
                for (int i = 0; i < maxip; ++i) queues[id][i] = null;
            }

            queues[id][ip] = new(queues[id][ip].Value.Item1 + num, high);
            Debug.Log($"Item:ip:{ip},Item:{id},{num},high:{high};\nqueues top of {id} : {(Tops(id) == null ? null : Tops(id).Value)}");
            return true;
        }
        //private bool UpdateTop(string id)
        //{
        //    if (Tops[id] != null) return false;
        //    for (int i = maxPriority - 1; i >= 0; --i)
        //    {
        //        if (queues[id][i].Count != 0)
        //        {
        //            Tops[id] = queues[id][i].Dequeue();
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        public (byte, int)? Answer(string id, int maxPackage = 1)
        {
            if (!queues.ContainsKey(id)) return null;

            //if (Tops(id) == null)
            //    if (!UpdateTop(id)) return null;


            (byte, int)? ans = null;// (queues[id][polling] == null) ? null : new(polling, queues[id][polling].Value.Item1);
            if (queues[id][polling] != null)
                if (queues[id][polling].Value.Item1 > maxPackage)
                {
                    ans = new(polling, maxPackage);
                    queues[id][polling] = new(queues[id][polling].Value.Item1 - maxPackage, queues[id][polling].Value.Item2);
                }
                else
                {
                    ans = new(polling, maxPackage);
                    queues[id][polling] = null;
                }
            int? maxh = null;
            for (int i = 0; i < maxip; ++i)
            {
                if (queues[id][i] != null)
                    if (maxh == null || maxh < queues[id][i].Value.Item2)
                        maxh = queues[id][i].Value.Item2;

            }
            if (maxh == null) return null;
            for (int _i = 0; _i < maxip; ++_i)
            {
                int i = (_i + polling + 1) % maxip;
                if (queues[id][i] != null && queues[id][i].Value.Item2 >= maxh)
                {
                    polling = (byte)i;
                }
            }


            return ans;
        }
    }

    /// <summary>
    /// 物流网络
    /// </summary>
    public class LogistNet
    {
        private InterFace manager;
        public InterFace Manager { get => manager; }
        private int maxIpNum = 8;
        private int count = 1;
        static private readonly int selfNetNum = 8;
        public readonly int id;
        static int nowid = 0;

        private bool threadPause = true;
        private bool threadAlive = true;
        private Thread thread;
        public List<LogistNetBlock> Blocks { get; } = new();
        public AskQueue AskQueue { get; } = new();
        public int Count { get => count; }
        public int MaxIpNum { get => maxIpNum; }

        private readonly LogistNetBlock[] builds = new LogistNetBlock[256];

        public LogistNet()
        {
            id = nowid;
            nowid++;

            //创建请求应答线程
            thread = new(RotateAnswer);
            thread.Start();
        }

        ~LogistNet()
        {
            threadAlive = false;
        }
        public void RotateAnswer()
        {
            while (threadAlive)
            {
                //土方法锁线程，待优化
                if (threadPause)
                {
                    string[] keys = AskQueue.Keys;
                    for (int i = 0; i < keys.Length; ++i)
                    {
                        if (keys[i] != null)
                            for (int j = 0; j < Blocks.Count; ++j)
                            {
                                if (!threadPause) break;
                                if (Blocks[j].Inter != null && !(AskQueue.Tops(keys[i]) != null && Blocks[j].Inter.Ip == AskQueue.Tops(keys[i]).Value.Item1))
                                {
                                    Blocks[j].Inter.AnswerLogist(keys[i]);
                                }
                            }
                        if (!threadPause) break;
                    }



                }
            }
        }

        public void UpdataAskQueue(int ip, Item item, int high = 0)
        {
            threadPause = false;


            AskQueue.UpdataAsk(ip, item, high);

            threadPause = true;
        }

        public void PushAskQueue(int ip, Item item, int high = 0)
        {
            threadPause = false;


            AskQueue.UpdataAsk(ip, item, high);

            threadPause = true;
        }

        public bool SetManager(InterFace mng)
        {
            if (mng == null)
            {
                maxIpNum = 0;
                return false;
            }
            if (mng.building is LogistCentral logist)
            {
                maxIpNum = logist.MaxIPNum();
                if (maxIpNum > 256) maxIpNum = 256;
            }
            else
            {
                maxIpNum = selfNetNum;
            }

            manager = mng;
            mng.SetIP(0);
            return true;
        }
        public bool SetIp(LogistNetBlock block)
        {
            int min = -1;
            for (int i = 1; i < maxIpNum; ++i)
            {
                if (builds[i] == null && min == -1)
                    min = i;
                if (block == builds[i])
                    return true;
            }

            if (min == -1) return false;
            if (block.Inter == null) return false;

            builds[min] = block;
            block.Inter.SetIP((byte)min);
            count += 1;
            Debug.Log(ToString());
            return true;
        }
        public bool DelIp(LogistNetBlock block)
        {
            int ip = block.Inter.Ip;
            if (builds[ip] == block)
            {
                builds[ip] = null;
                --count;
                return true;
            }
            return false;
        }

        public void Marge(LogistNet net)
        {
            if (net == this) return;
            //for (int i = 0; i < net.maxIpNum; ++i)
            //{
            //	if (net.builds[i] != null)
            //	{
            //bool flg = true;
            //for (int j = 0; j < maxIpNum; ++j)
            //{
            //	if (net.builds[i] == builds[i])
            //	{
            //		flg = false;
            //		break;
            //	}
            //}

            //		if (SetIp(net.builds[i]))
            //		{
            //			net.builds[i].ParentNet = this;
            //		}
            //	}
            //}
            foreach (var i in net.Blocks)
            {
                if (!Blocks.Contains(i))
                {
                    Blocks.Add(i);
                    i.ParentNet = this;
                    this.SetIp(i);
                }
            }
            net.Blocks.Clear();
            foreach (var i in Blocks)
            {
                if (i.Inter != null) i.Inter.SendRouter();
            }

            //for (int i = 0; i < maxIpNum; ++i)
            //{
            //	if (builds[i] != null)
            //		builds[i].Inter.SendRouter();
            //}
        }

        public InterFace GetInterFace(int ip)
        {
            if (ip > 255 || ip < 0) return null;
            return builds[ip] != null ? builds[ip].Inter : null;
        }

        public static int BuildSum(LogistNet net1, LogistNet net2)
        {
            int sum = net1.count + net2.count - 1;
            for (int i = 1; i < net1.maxIpNum; ++i)
            {
                for (int j = i; j < net2.maxIpNum; ++j)
                {
                    if (net1.builds[i] == net2.builds[j]) --sum;
                }
            }
            return sum;
        }

        public override string ToString()
        {
            string ans = $"No.{id} Net:\n";
            ans += "The ip table:{\n";
            for (int i = 0; i < 256; ++i)
            {
                ans += $"ip:{i}->inter:{(builds[i] == null ? "null" : $"No.{builds[i].id} Inter")}\n";
            }

            return ans;
        }
    }

    public class LogistNetBlock
    {
        public readonly int id;
        static int nowid = 0;
        readonly List<LogistPipe> pipes = new();
        public LogistNet ParentNet { get; set; } = new();
        public int Count { get => pipes.Count; }
        public InterFace Inter { get; set; } = null;
        public LogistNetBlock()
        {
            id = nowid;
            nowid++;
            ParentNet.Blocks.Add(this);
        }
        public void Add(LogistPipe pipe)
        {
            pipes.Add(pipe);
        }

        public void Remove(LogistPipe pipe)
        {
            pipes.Remove(pipe);
        }

        public void Marge(LogistNetBlock netblock)
        {
            if (netblock.pipes.Count == 0) return;
            if (netblock == this) return;
            if (netblock.Inter != null && this.Inter != null && this.ParentNet != null)
            {
                if (!ParentNet.SetIp(netblock)) throw new System.Exception("out of size!");
                else Debug.LogWarning("marge networking");
            }
            foreach (var i in netblock.pipes)
            {
                pipes.Add(i);
                i.ParentLogist = this;
            }
            netblock.pipes.Clear();
        }

    }

    /// <summary>
    /// 能量网络
    /// </summary>
    public class EnergyNet
    {
        public float power;
        public float load;

    }

    /// <summary>
    /// 库存
    /// </summary>
    public class Inventory
    {
        private int maxCount = 0;
        private int count = 0;

        private Dictionary<string, int> items;

        public Dictionary<string, int> Items { get => items; }
        public int Count { get { if (maxCount == 0) return 0; else return count; } }
        public int MaxCount { get { return maxCount; } }

        public Inventory(int size)
        {
            maxCount = size;
            items = new();
        }
        public int Contains(string id)
        {
            //if (items == null) Debug.Log("这里怎么能是null呢？这不能啊");
            //待优化
            try
            {
                if (items.TryGetValue(id, out int ans))
                    return ans;
                else
                    return -1;
            }
            catch (System.NullReferenceException)
            {
                return -1;
            }
        }
        public bool Input(string id, int number)
        {
            if (maxCount == 0) return false;
            if (count + number > maxCount)
                return false;
            if (items.ContainsKey(id))
            {
                items[id] += number;
                count += number;
            }
            else
            {
                items[id] = number;
                count += number;
            }
            return true;
        }
        public Item Output(string id, int number)
        {
            if (maxCount == 0) return null;
            if (items.ContainsKey(id))
            {
                if (items[id] > number)
                {
                    items[id] -= number;
                    count -= number;
                }
                else if (items[id] == number)
                {
                    if (items.Remove(id))
                        count -= number;
                }
                else
                {
                    number = items[id];
                    if (items.Remove(id))
                        count -= number;
                }
            }
            else
                return null;
            return new Item(id, number);
        }

        public int Get(string id)
        {
            return items[id];
        }

        public string GetLog()
        {
            if (items == null) return "null";
            string s = "{";
            foreach (var i in items)
            {
                s += i.Key + " : " + i.Value + " , ";
            }
            s += "}";
            return s;
        }
    }

}