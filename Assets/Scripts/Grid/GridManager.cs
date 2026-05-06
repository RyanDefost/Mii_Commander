using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Grid
{
    [RequireComponent(typeof(GridGenerator))]
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private GridGenerator generator;
        private readonly List<GridInstance> gridInstances = new();
        private List<Vector3?> availablePositions;
        private List<Vector3?> availableOffGridPositions;
        public Vector2 CellSize { get => this.generator.cellSize; private set => this.generator.cellSize = value; }

        public class GridInstance
        {
            public Vector3 position;
            public readonly int index;
            public readonly GameObject gameObj;
            public bool offGrid;

            public GridInstance(GameObject gameObj, int foundIndex, GridGenerator generator, bool offGrid)
            {
                this.gameObj = gameObj;
                this.position = !offGrid ? generator.GetPosAt(foundIndex) : generator.GetOffGridPosAt(foundIndex);
                this.index = foundIndex;
                this.offGrid = offGrid;
            }
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.generator = GetComponent<GridGenerator>();
        }

        private void Start()
        {
            ComponentRegistry.AddToRegistry(this);
            this.availableOffGridPositions ??= this.generator.GetAllOffGridPositions();
            this.availablePositions ??= this.generator.GetAllPositions();
        }

        private void OnDestroy() => ComponentRegistry.RemoveFromRegistry(this);

        public GridInstance GetNearestPosition(Vector3 position, GameObject gameObj, GridInstance previous = null)
        {
            Vector3? posToIgnore = null;
            bool isBottomRow = false;
            if (previous != null)
            {
                isBottomRow = previous.index < this.generator.Width;
                posToIgnore = previous.position;
            }
            
            if (!isBottomRow)
                return GetNearestGridPosition(position, gameObj, posToIgnore);
            return GetNearestOffGridPosition(position, gameObj, posToIgnore, previous.offGrid);
        }

        public GridInstance GetNearestOffGridPosition(Vector3 position, GameObject gameObj, Vector3? posToIgnore = null, bool isAlreadyOffGrid = false)
        {
            int foundIndexOffGrid = GetNearestIndex(position, this.availableOffGridPositions, posToIgnore);
            int foundIndex = GetNearestIndex(position, this.availablePositions, posToIgnore);
            
            if (foundIndexOffGrid == -1 && !isAlreadyOffGrid)
                return RegisterGridPosition(gameObj, foundIndex);
            if (isAlreadyOffGrid)
                return null;
            
            this.availableOffGridPositions[foundIndexOffGrid] = null;
            GridInstance newInstance = new(gameObj, foundIndexOffGrid, this.generator, true);
            this.gridInstances.Add(newInstance);
            return newInstance;
        }

        public GridInstance GetNearestGridPosition(Vector3 position, GameObject gameObj, Vector3? posToIgnore = null)
        {
            int foundIndex = GetNearestIndex(position, this.availablePositions, posToIgnore);
            return foundIndex == -1 ? null : RegisterGridPosition(gameObj, foundIndex);
        }

        private GridInstance RegisterGridPosition(GameObject gameObj, int foundIndex)
        {
            this.availablePositions[foundIndex] = null;
            GridInstance newInstance = new(gameObj, foundIndex, this.generator, false);
            this.gridInstances.Add(newInstance);
            return newInstance;
        }

        private static int GetNearestIndex(Vector3 position, List<Vector3?> positions, Vector3? posToIgnore = null)
        {
            float smallestDistance = float.MaxValue;
            int foundIndex = -1;
            for (int i = 0; i < positions.Count; i++)
            {
                Vector3? availablePosition = positions[i];
                if (availablePosition == null || (posToIgnore.HasValue && availablePosition.Value == posToIgnore.Value)) continue;
                float distance = Vector3.Distance(position, availablePosition.Value);
                if (!(distance <= smallestDistance)) continue;
                smallestDistance = distance;
                foundIndex = i;
            }
            return foundIndex;
        }

        public void ReleaseInstance(GridInstance target)
        {
            if (!target.offGrid)
            {
                if (target.index >= this.availablePositions.Count || target.index < 0)
                    return;
                this.availablePositions[target.index] = target.position;
            }
            else
            {
                if (target.index >= this.availableOffGridPositions.Count || target.index < 0)
                    return;
                this.availableOffGridPositions[target.index] = target.position;
            }

            this.gridInstances.RemoveSwapBack(target);
        }

        public void ForAllGridItems(Action<GridInstance> action)
        {
            List<GridInstance> tempGridInstances = new(this.gridInstances);
            tempGridInstances.Sort((a, b) => a.position.y.CompareTo(b.position.y));
            tempGridInstances.ForEach(action);
        }
    }
}