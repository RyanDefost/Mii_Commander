using System.Collections.Generic;
using Grid;
using UnityEngine;

namespace Synergy
{
    [CreateAssetMenu(fileName = "Synergy", menuName = "ScriptableObjects/Synergy", order = 1)]
    public class Synergy : ScriptableObject
    {
        public List<BoardItem> synergyItems = new();
        
        [Header("Synergy info")]
        public GameObject output;
        public bool hasOrder = true;
    }
}