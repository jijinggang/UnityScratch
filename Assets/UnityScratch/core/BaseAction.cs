using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UnityScratch.core
{
    public abstract class BaseAction : ScriptableObject 
    {
        protected GameObject _go = null;
        public float x { get; set; }
        public float y { get; set; }
        void Start()
        {
            //_go = gameObject;
        }
        public abstract void Run();
        public abstract string Id();
        public abstract BaseAction Create();

        public virtual string Icon() { return ""; }
    }
}
