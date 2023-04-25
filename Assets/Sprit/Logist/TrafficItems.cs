using System.Collections;
using UnityEngine;

using GameBase;
using Logist;

namespace Logist
{
    public class TrafficItems : MonoBehaviour
    {
        Item item = null;
        byte toip;
        Dircation nowDir;

        float stllTime;
        float beginTime;
        InterFace toInter;
        InterFace baseInter;
        static float speed = 1;
        static int[] step = { 0, 0, -1, 1, 1, -1, 0, 0 };

        Vector2Int nowvec = new();

        SpriteRenderer spr;

        public void Init(string id, int count, byte ip, InterFace self)
        {
            Init(new(id, count), ip, self.dir, self.Router.Len(ip), self.ParentLogist.ParentNet.GetInterFace(ip), self);
        }
        public void Init(Item it, byte ip, InterFace self)
        {
            Init(it, ip, self.dir, self.Router.Len(ip), self.ParentLogist.ParentNet.GetInterFace(ip), self);
        }
        private void Init(Item it, byte ip, Dircation firstDir, int lengh, InterFace inter, InterFace self)
        {
            toip = ip;
            item = it;
            nowDir = firstDir;
            beginTime = Time.time;
            stllTime = (lengh - 1) / speed;
            toInter = inter;

            transform.position = self.transform.position;
            baseInter = self;

            spr = gameObject.AddComponent<SpriteRenderer>();
            spr.sprite = Manager.LogistManager.PeakageImage(it.id);
            transform.localScale = new(0.3f, 0.3f, 0.3f);
        }

        // Use this for initialization
        private void Start()
        {
            if (item == null || toInter == null)
                DestroyTrafficItems();

        }

        // Update is called once per frame
        private void Update()
        {
            float v = speed * Time.deltaTime;
            transform.position = new(transform.position.x + v * step[(int)nowDir],
                transform.position.y + v * step[(int)nowDir + 4],
                transform.position.z);
        }

        private void FixedUpdate()
        {
            if (Time.time - beginTime >= stllTime)
            {
                if (!toInter.InputItem(item))
                {
                    baseInter.building.Invent.CompelInput(item.id, item.count);
                    Debug.Log($" {item.id}-{item.count} back! ");
                }
                //等待对象池优化
                Destroy(gameObject);
            }
            if (Mathf.FloorToInt(transform.position.x - step[(int)nowDir] * 0.5f) != nowvec.x
                || Mathf.FloorToInt(transform.position.y - step[(int)nowDir + 4] * 0.5f) != nowvec.y)
            {
                nowvec.x = Mathf.FloorToInt(transform.position.x - step[(int)nowDir] * 0.5f);
                nowvec.y = Mathf.FloorToInt(transform.position.y - step[(int)nowDir + 4] * 0.5f);
                Block ob = Manager.MapManager.GetBlock(nowvec);
                if (ob is LogistPipe pipe)
                {
                    if (pipe.Router != null)
                    {
                        nowDir = pipe.Router.Dir(toip);
                    }
                }
            }
        }

        private void DestroyTrafficItems()
        {
            Destroy(gameObject);
        }
    }
}