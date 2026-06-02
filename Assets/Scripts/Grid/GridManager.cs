using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    /// <summary>
    /// Manages everything in the grid
    /// </summary>
    [RequireComponent(typeof(GridGenerator))]
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private GridGenerator generator;
        private readonly Dictionary<GridIndex, GridInstance> activeGrid = new();
        private List<Vector3?> openOnGridPositions;
        private List<Vector3?> openOffGridPositions;
        public Vector2 CellSize { get => this.generator.cellSize; private set => this.generator.cellSize = value; }

        /// <summary>
        /// Represents a place inside the grid
        /// </summary>
        public readonly struct GridIndex : IComparable, IEquatable<GridIndex>
        {
            public readonly int index;
            public readonly bool isOffGrid;

            public GridIndex(int index, bool isOffGrid)
            {
                this.index = index;
                this.isOffGrid = isOffGrid;
            }

            public int CompareTo(object obj)
            {
                if (obj is GridIndex indexObj)
                    return this.index.CompareTo(indexObj.index) + this.isOffGrid.CompareTo(indexObj.isOffGrid);
                return 0;
            }

            public bool Equals(GridIndex other) => 
                this.index == other.index && this.isOffGrid == other.isOffGrid;

            public override bool Equals(object obj) => 
                obj is GridIndex other && Equals(other);

            public override int GetHashCode() => 
                HashCode.Combine(this.index, this.isOffGrid);
        }
        
        /// <summary>
        /// Represents a taken grid position, and houses needed references to the item inside
        /// </summary>
        public class GridInstance
        {
            public Vector3 position;
            public readonly GridIndex index;
            public readonly GameObject gameObj;
            public readonly BoardItem boardItem;
            public readonly GridMoveable moveable;

            public GridInstance(GameObject gameObj, GridIndex index, GridGenerator generator, BoardItem boardItem, GridMoveable moveable)
            {
                this.gameObj = gameObj;
                this.position = generator.GetPosAt(index);
                this.index = index;
                this.boardItem = boardItem;
                this.moveable = moveable;

                if (this.boardItem)
                    this.boardItem.gridInstanceRef = this;
            }
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.generator = GetComponent<GridGenerator>();
        }

        private void Awake() => ComponentRegistry.AddToRegistry(this);

        private void Start()
        {
            this.openOffGridPositions ??= this.generator.GetAllOffGridPositions();
            this.openOnGridPositions ??= this.generator.GetAllPositions();
        }

        private void OnDestroy() => ComponentRegistry.RemoveFromRegistry(this);

        /// <summary>Tries to get a near available position both on grid and off grid</summary>
        public GridInstance GetNearestPosition(Vector3 position, GameObject gameObj, BoardItem item, GridInstance previous = null)
        {
            Vector3? posToIgnore = null;
            bool isBottomRow = false;
            if (previous != null)
            {
                isBottomRow = previous.index.index < this.generator.Width;
                posToIgnore = previous.position;
            }
            
            if (!isBottomRow)
                return GetNearestGridPosition(position, gameObj, item, posToIgnore);
            return GetNearestOffGridPosition(position, gameObj, item, posToIgnore, previous.index.isOffGrid);
        }

        /// <summary>Used in case the grid is full, a last row underneath the board. Try to get a available position</summary>
        public GridInstance GetNearestOffGridPosition(Vector3 position, GameObject gameObj, BoardItem item,
            Vector3? posToIgnore = null, bool isAlreadyOffGrid = false)
        {
            int foundIndexOffGrid = GetNearestIndex(position, this.openOffGridPositions, posToIgnore);
            int foundIndex = GetNearestIndex(position, this.openOnGridPositions, posToIgnore);
            
            if (foundIndexOffGrid == -1 && !isAlreadyOffGrid)
                return RegisterGridPosition(gameObj, item, foundIndex);
            if (isAlreadyOffGrid)
                return null;
            
            GridInstance newInstance = new(gameObj,
                new GridIndex(foundIndexOffGrid, true),
                this.generator,
                item,
                item?.GetBoardComponent<GridMoveable>());
            return RegisterInGrid(newInstance);
        }

        /// <summary>Tries to find the nearest available position on the board</summary>
        public GridInstance GetNearestGridPosition(Vector3 position, GameObject gameObj, BoardItem item,
            Vector3? posToIgnore = null)
        {
            int foundIndex = GetNearestIndex(position, this.openOnGridPositions, posToIgnore);
            return foundIndex == -1 ? null : RegisterGridPosition(gameObj, item, foundIndex);
        }

        /// <summary>
        /// Gets an array of neighbors, based on a given pattern
        /// </summary>
        /// <param name="instance">mid-point of the pattern</param>
        /// <param name="pattern">pattern around the desired</param>
        /// <returns>an array of found neighbors and empty cells</returns>
        public GridInstance[] GetNeighbors(GridInstance instance, NeighborPattern pattern)
        {
            int distanceToLeft = instance.index.index % this.generator.Width;
            int distanceToRight = (this.generator.Width - 1) - distanceToLeft;
            
            GridInstance[] result = new GridInstance[pattern.width * pattern.height];

            int centerX = pattern.center.x;
            int centerY = pattern.center.y;
            
            foreach (Vector2Int pos in pattern.positions)
            {
                if (pos.x < -distanceToLeft || pos.x > distanceToRight || pos == Vector2Int.zero) continue; // outside of grid

                int index = pos.x + (pos.y * this.generator.Width) + instance.index.index;
                if (instance.index.isOffGrid)
                    index += this.openOnGridPositions.Count;
                
                bool isOffGrid = false;
                if (index >= this.openOnGridPositions.Count)
                {
                    index -= this.openOnGridPositions.Count;
                    isOffGrid = true;
                }
                
                if ((isOffGrid && index >= this.openOffGridPositions.Count) || index < 0) // outside of grid
                    continue;

                int baseIndex = (pos.x + centerX) + (pos.y + centerY) * pattern.width;
                switch (isOffGrid) // check if position is taken, return neighbor if position was taken
                {
                    case true when this.openOffGridPositions[index] == null:
                        result[baseIndex] = this.activeGrid[new GridIndex(index, true)];
                        continue;
                    case false when this.openOnGridPositions[index] == null:
                        result[baseIndex] = this.activeGrid[new GridIndex(index, false)];
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a grid instance, and makes sure the position isn't taken again
        /// </summary>
        private GridInstance RegisterGridPosition(GameObject gameObj, BoardItem item, int index)
        {
            return RegisterInGrid(new GridInstance(gameObj,
                new GridIndex(index, false),
                this.generator,
                item,
                item?.GetBoardComponent<GridMoveable>()));
        }

        /// <summary>
        /// Registers a grid instance, making sure to occupy the space inside the grid.
        /// </summary>
        /// <param name="instance">instance to register</param>
        /// <returns>the instance at the desired location, will return already existing obj if place already taken</returns>
        public GridInstance RegisterInGrid(GridInstance instance)
        {
            this.activeGrid.TryGetValue(instance.index, out GridInstance found);
            if (found != null) return found;
            
            if (!instance.index.isOffGrid)
                this.openOnGridPositions[instance.index.index] = null;
            else
                this.openOffGridPositions[instance.index.index] = null;
            
            this.activeGrid[instance.index] = instance;
            return instance;
        }

        /// <summary>
        /// Gets the nearest position from a collection of positions
        /// </summary>
        /// <param name="position">position to check near</param>
        /// <param name="positions">collection to look through</param>
        /// <param name="posToIgnore">optional position to ignore from the collection</param>
        /// <returns>index of nearest position</returns>
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

        /// <summary>
        /// Removes a taken position in the grid, and makes it a available once more
        /// </summary>
        public void ReleaseInstance(GridInstance target)
        {
            if (!target.index.isOffGrid)
            {
                if (target.index.index >= this.openOnGridPositions.Count || target.index.index < 0)
                    return;
                this.openOnGridPositions[target.index.index] = target.position;
            }
            else
            {
                if (target.index.index >= this.openOffGridPositions.Count || target.index.index < 0)
                    return;
                this.openOffGridPositions[target.index.index] = target.position;
            }

            this.activeGrid.Remove(target.index);
        }

        /// <summary>
        /// A method to call a method on all taken grid objects
        /// </summary>
        /// <param name="action">to invoke on all taken positions</param>
        public void ForAllGridItems(Action<GridInstance> action)
        {
            List<GridInstance> tempGridInstances = new(this.activeGrid.Values);
            tempGridInstances.Sort((a, b) => a.position.y.CompareTo(b.position.y));
            tempGridInstances.ForEach(action);
        }
    }
}