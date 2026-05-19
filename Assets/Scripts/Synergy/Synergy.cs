using System;
using System.Collections.Generic;
using UnityEngine;

namespace Synergy
{
    [CreateAssetMenu(fileName = "SynergyData", menuName = "ScriptableObjects/Synergy", order = 1)]
    public class Synergy : ScriptableObject, ISynergy
    {
        public List<SynergyItem>  synergyItems = new List<SynergyItem>();
        
        [Header("Synergy settings")]
        public bool requiredInOrder = false;
    }

    [Serializable]
    public struct SynergyItem
    {
        public BoardItem BoardItem;

        public bool staticItem;
    }
}