using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Grid
{
    [RequireComponent(typeof(GridGenerator))]
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private GridGenerator generator;
        [NonSerialized]
        public List<GridInstance> gridInstances = new();
        private List<Vector3?> availablePositions;

        public class GridInstance
        {
            public Vector3 position;
            public int index;
            public Interactable interactable;

            public GridInstance(Interactable interactable, int foundIndex, GridGenerator generator)
            {
                this.interactable = interactable;
                this.position = generator.GetPosAt(foundIndex);
                this.index = foundIndex;
            }
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.generator = GetComponent<GridGenerator>();
        }

        private void Start() => ComponentRegistry.AddToRegistry(this);
        private void OnDestroy() => ComponentRegistry.RemoveFromRegistry(this);

        public GridInstance GetNearestGridPosition(Vector3 position, Interactable interactable)
        {
            this.availablePositions ??= this.generator.GetAllPositions();
            
            float smallestDistance = float.MaxValue;
            int foundIndex = -1;
            for (int i = 0; i < this.availablePositions.Count; i++)
            {
                Vector3? availablePosition = this.availablePositions[i];
                if (availablePosition == null) continue;
                float distance = Vector3.Distance(position, availablePosition.Value);
                if (!(distance <= smallestDistance)) continue;
                smallestDistance = distance;
                foundIndex = i;
            }
            
            if (foundIndex == -1) return null;
            
            this.availablePositions[foundIndex] = null;
            GridInstance newInstance = new(interactable, foundIndex, this.generator);
            this.gridInstances.Add(newInstance);
            return newInstance;

        }

        public void ReleaseInstance(GridInstance target)
        {
            if (target.index >= this.gridInstances.Count || target.index < 0)
                return;
            this.availablePositions[target.index] = target.position;
            this.gridInstances.RemoveSwapBack(target);
        }
    }
}