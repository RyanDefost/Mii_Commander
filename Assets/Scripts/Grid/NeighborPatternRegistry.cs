using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grid
{
    public class NeighborPatternRegistry : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private List<PatternInstance> patterns = new();
        [SerializeField]
        private Color neighborColor;
        [SerializeField]
        private Color centerColor;

        [Serializable]
        public class PatternInstance
        {
            public string name;
            public Texture2D texture;
        }
#endif
        [SerializeField, HideInInspector]
        private List<LivePattern> usedPatterns = new();
        
        [Serializable]
        private class LivePattern
        {
            public string name;
            public NeighborPattern pattern;

            public LivePattern(string name, Texture2D tex, Color neighborColor, Color centerColor)
            {
                this.name = name;
                this.pattern = new NeighborPattern(tex, neighborColor, centerColor);
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            this.usedPatterns.Clear();
            foreach (PatternInstance instance in 
                     this.patterns.Where(instance => instance.name != "" && instance.texture != null))
                this.usedPatterns.Add(new LivePattern(instance.name, instance.texture, this.neighborColor, this.centerColor));
        }

        private void Awake() => ComponentRegistry.AddToRegistry(this);

        /// <summary>
        /// Gets a struct containing a pattern, make sure to reget if the data changes inside the registry
        /// </summary>
        public NeighborPattern? GetNeighborPattern(string patternName) =>
            this.usedPatterns.FirstOrDefault(a => a.name == patternName)?.pattern;
    }
}