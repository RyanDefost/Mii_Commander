using System;
using System.Collections.Generic;
using UnityEngine;

public class CandyTypeLookup : MonoBehaviour
{
    [SerializeField]
    private List<CandyTypeOption> options = new();
    
    [Serializable]
    public class CandyTypeOption
    {
        public string name;
        public GameObject candyPrefab;
        public int index;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        for (int i = 0; i < this.options.Count; i++)
        {
            CandyTypeOption option = this.options[i];
            option.index = i;
        }
    }

    private void Awake() => ComponentRegistry.AddToRegistry(this);
    public CandyTypeOption Get(int typeIndex) => this.options[typeIndex];
}