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
        private Color toCheckColor;

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

            public LivePattern(string name, Texture2D tex, Color toCheckColor)
            {
                this.name = name;
                this.pattern = new NeighborPattern(tex, toCheckColor);
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            this.usedPatterns.Clear();
            foreach (PatternInstance instance in 
                     this.patterns.Where(instance => instance.name != "" && instance.texture != null))
                this.usedPatterns.Add(new LivePattern(instance.name, instance.texture, this.toCheckColor));
        }

        private void Start() => ComponentRegistry.AddToRegistry(this);

        public NeighborPattern? GetNeighborPattern(string patternName) =>
            this.usedPatterns.FirstOrDefault(a => a.name == patternName)?.pattern;
    }
}