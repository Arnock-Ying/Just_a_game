using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Manager
{
    public class LogistManager : MonoBehaviour
    {

        private static LogistManager instend = null;
        public static LogistManager Instend { get { return instend; } }

        [SerializeField]
        private Sprite[] pipeImage;
        [SerializeField]
        private Sprite[] peakageImage;
        [SerializeField]
        private string[] itemNumToId;
        public static Sprite[] PipeImage { get { return instend.pipeImage; } }
        public static Sprite PeakageImage(string id)
        {
            for (int i = 0; i < instend.itemNumToId.Length; ++i)
            {
                if (instend.itemNumToId[i] == id)
                    return instend.peakageImage[i];
            }
            return null;
        }
        public void Awake()
        {
            if (instend == null)
                instend = this;
            else
                Destroy(gameObject);


        }
    }
}