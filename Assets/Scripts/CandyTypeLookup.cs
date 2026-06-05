using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lookup table for all candy types, and their data
/// Possible to change per level since it's a registered component
/// </summary>
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
    private void OnDestroy() => ComponentRegistry.RemoveFromRegistry(this);

    /// <summary>
    /// Exposes candy from lookup table
    /// </summary>
    /// <param name="typeIndex">index of candy wish to get</param>
    /// <returns>requested candy</returns>
    public CandyTypeOption Get(int typeIndex) => this.options[typeIndex];
}