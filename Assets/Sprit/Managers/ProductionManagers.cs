using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameBase;

namespace Manager
{
    public class ProductionManager : MonoBehaviour
    {
        private List<Formula> formulas = new();
        private static ProductionManager instend = null;
        public static ProductionManager Instend { get { return instend; } }
        public static List<Formula> Formulas { get { return instend.formulas; } }

        public void Awake()
        {
            if (instend == null)
                instend = this;
            else
                Destroy(gameObject);
        }
    }
}