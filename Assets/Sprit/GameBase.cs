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
                    pipe1.BuildPipe(false);
                }
                if (MapManager.GetBlock(i, pos.y + Mathf.CeilToInt(size.y / 2.0f)) is LogistPipe pipe2)
                {
                    pipe2.BuildPipe(false);
                }
            }
            for (int i = pos.y - Mathf.FloorToInt(size.y / 2.0f); i < pos.y + Mathf.CeilToInt(size.y / 2.0f); ++i)
            {
                if (MapManager.GetBlock(pos.x - Mathf.FloorToInt(size.x / 2.0f) - 1, i) is LogistPipe pipe1)
                {
                    pipe1.BuildPipe(false);
                }
                if (MapManager.GetBlock(pos.x + Mathf.CeilToInt(size.x / 2.0f), i) is LogistPipe pipe2)
                {
                    pipe2.BuildPipe(false);
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
        protected Inventory invent = new();
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

        protected void Start()
        {
            maxInvent = 100;
            invent = new();
            Item wood = new("wood", 1);//临时用用
            List<Item> temp = new();
            temp.Add(wood);
            formula = new(temp);
        }

        protected void Update()
        {
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
                                iface.AskLogist(new Item(it.id, maxInvent));
                            }
                            debug = "请求: " + it.id + " " + maxInvent + "\n内存物品数量: " + invent.Contains(it.id);
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
        Dictionary<string, Queue<KeyValuePair<byte, int>>[]> queues = new();
        Dictionary<string, KeyValuePair<byte, int>?> tops = new();
        public Dictionary<string, KeyValuePair<byte, int>?> Tops { get => tops; }

        public bool Push(int ip, Item item, int high = 0)
        {
            return Push(ip, item.id, item.count, high);
        }
        private bool Push(int ip, string id, int num, int high = 0)
        {
            Debug.Log($"ip:{ip},Item:{id},{num},high:{high}");
            if (ip >= 256 || ip < 0) return false;
            if (num < 0) return false;
            if (high >= maxPriority || high < 0) return false;
            if (!queues.ContainsKey(id))
            {
                queues.Add(id, new Queue<KeyValuePair<byte, int>>[maxPriority]);
                tops.Add(id, null);
                for (int i = 0; i < maxPriority; ++i)
                {
                    queues[id][i] = new();
                }
            }
            queues[id][high].Enqueue(new((byte)ip, num));
            UpdateTop(id);
            Debug.Log($"queues top of {id} : {tops[id].Value}");
            return true;
        }

        private bool UpdateTop(string id)
        {
            for (int i = maxPriority - 1; i >= 0; --i)
            {
                if (queues[id][i].Count != 0)
                {
                    tops[id] = queues[id][i].Dequeue();
                    return true;
                }
            }
            return false;
        }
        public KeyValuePair<byte, int>? Answer(string id, int maxPackage = 1)
        {
            if (!queues.ContainsKey(id)) return null;
            if (tops[id] == null)
                if (!UpdateTop(id)) return null;


            KeyValuePair<byte, int>? ans = null;
            if (tops[id].Value.Value > maxPackage)
            {
                ans = new(tops[id].Value.Key, maxPackage);
                tops[id] = new(tops[id].Value.Key, tops[id].Value.Value - maxPackage);
            }
            else
            {
                ans = tops[id];
                tops[id] = null;
                UpdateTop(id);
                
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
                    try
                    {
                        foreach (var i in AskQueue.Tops)
                        {
                            if (i.Value != null)
                                foreach (var j in Blocks)
                                {
                                    if (!threadPause) break;
                                    if (j.Inter != null)
                                    {
                                        j.Inter.AnswerLogist(i.Key);
                                    }
                                }
                            if (!threadPause) break;
                        }
                    }
                    catch(System.InvalidOperationException)
                    {
                        Debug.Log("out");
                    }
                    

                }
            }
        }

        public void PushAskQueue(int ip, Item item, int high = 0)
        {
            threadPause = false;


            AskQueue.Push(ip, item, high);

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
            return builds[ip].Inter;
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

        private Dictionary<string, int> items = new();

        public Dictionary<string, int> Items { get => items; }
        public int Count { get { if (maxCount == 0) return 0; else return count; } }
        public int MaxCount { get { return maxCount; } }
        public Inventory()
        {
            maxCount = 0;
        }
        public Inventory(int size)
        {
            maxCount = size;
            items = new();
        }
        public int Contains(string s)
        {
            if (items.ContainsKey(s))
                return items[s];
            else
                return -1;
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
                    return null;
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