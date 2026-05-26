using System.Collections.Generic;
using Grid;
using UnityEngine;

namespace Synergy
{
    [CreateAssetMenu(fileName = "Synergy", menuName = "ScriptableObjects/Synergy", order = 1)]
    public class Synergy : ScriptableObject
    {
        public string patternName;
        public List<BoardItem> synergyItems = new List<BoardItem>();

        public bool hasOrder;
    }
}