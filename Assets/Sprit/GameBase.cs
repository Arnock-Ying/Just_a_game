using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase
{
    ///<summary>
    ///方块类
    ///</summary>
    public class Block : MonoBehaviour
    {
        //protected int pos_x, pos_y;
        protected int hp;

        public bool Dismantle()
        {
            return false;
        }
    }

    /// <summary>
    /// 建筑基类
    /// </summary>
    public class BaseBuild : Block
    {
        private int localip;
        private LogistNet fatherLogist = null;
        private EnergyNet fatherEngrgy;
        private Inventory invent = null;

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public string GetIP()
        {
            if (fatherLogist == null)
                return localip.ToString();
            else
                return fatherLogist.manager.GetIP() + '.' + localip;
        }
    }

    ///<summary>
    ///物流管道
    /// </summary>
    public class LogistPipe : Block
    {

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

        Item(string _id = "", int _count = 0)
        {
            id = _id;
            count = _count;
        }
        Item() { id = ""; count = 0; }
    }

    /// <summary>
    /// 物流网络
    /// </summary>
    public class LogistNet
    {
        public LogistCentral manager;
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
        private int maxCount;
        private int count;

        private Item[] items;

        public int Count { get { return count; } }
        public int MaxCount { get { return maxCount; } }
    }

}