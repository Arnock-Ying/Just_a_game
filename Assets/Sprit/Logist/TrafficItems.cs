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
        static float speed = 1;
        static int[] step = { 0, 0, -1, 1, 1, -1, 0, 0 };

        public void Init(string id, int count, byte ip, InterFace self)
        {
            Init(new(id, count), ip, self.dir, self.Router.Len(ip), self.ParentLogist.ParentNet.GetInterFace(ip));
        }
        public void Init(Item it, byte ip, InterFace self)
        {
            Init(it, ip, self.dir, self.Router.Len(ip), self.ParentLogist.ParentNet.GetInterFace(ip));
        }
        private void Init(Item it, byte ip, Dircation firstDir, int lengh, InterFace inter)
        {
            toip = ip;
            item = it;
            nowDir = firstDir;
            beginTime = Time.time;
            stllTime = lengh / speed;
            toInter = inter;
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

            }
        }

        private void DestroyTrafficItems()
        {
            Destroy(gameObject);
        }
    }
}