using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBase
{
    ///<summary>
    ///������
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
    /// ��������
    /// </summary>
    public class BaseBuild : Block
    {
        private int localip;
        private LogistNet fatherLogist = null;
        private EnergyNet fatherEngrgy;
        private Inventory invent = null;

        /// <summary>
        /// ��ȡIP
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
    ///�����ܵ�
    /// </summary>
    public class LogistPipe : Block
    {

    }

    ///<summary>
    ///�����ܵ�
    ///</summary>
    public class EnergyPipe : Block
    {

    }

    ///<summary>
    ///����
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
    /// ��Ʒ��
    /// </summary>
    public class Item
    {
        public string id;
        public int count;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public class LogistNet
    {
        public LogistCentral manager;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public class EnergyNet
    {
        public float power;
        public float load;

    }

    /// <summary>
    /// ���
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