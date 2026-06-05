using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    [Serializable]
    public struct GridCellPass
    {
        public string name;
        
        public Material material;
        public List<int> positionIndexes;
    }
}