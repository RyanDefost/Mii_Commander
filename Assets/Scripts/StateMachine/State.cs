using System;
using UnityEngine;

namespace Managers
{
    [System.Serializable]
    public abstract class State<T>
    {
        public T Owner { get; set; }
        
        public abstract void Start();
        public abstract void Update();
        public abstract void Exit();
    }
}