using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Grid
{
    /// <summary>
    /// Manages everything inside of the grid
    /// </summary>
    [RequireComponent(typeof(GridGenerator))]
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private GridGenerator generator;
        private readonly List<GridInstance> gridInstances = new();
        private List<Vector3?> availablePositions;
        private List<Vector3?> availableOffGridPositions;
        public Vector2 CellSize { get => this.generator.cellSize; private set => this.generator.cellSize = value; }

        /// <summary>
        /// Representation of a taken grid position
        /// </summary>
        public class GridInstance
        {
            public Vector3 position;
            public readonly int index;
            public readonly GameObject gameObj;
            public readonly BoardItem boardItem;
            public readonly GridMoveable moveable;
            public readonly bool offGrid;

            public GridInstance(GameObject gameObj, int foundIndex, GridGenerator generator, bool offGrid, BoardItem boardItem, GridMoveable moveable)
            {
                this.gameObj = gameObj;
                this.position = !offGrid ? generator.GetPosAt(foundIndex) : generator.GetOffGridPosAt(foundIndex);
                this.index = foundIndex;
                this.offGrid = offGrid;
                this.boardItem = boardItem;
                this.moveable = moveable;
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
            this.availableOffGridPositions ??= this.generator.GetAllOffGridPositions();
            this.availablePositions ??= this.generator.GetAllPositions();
            ComponentRegistry.AddToRegistry(this);
        }

        private void OnDestroy() => ComponentRegistry.RemoveFromRegistry(this);

        /// <summary>Tries to get a near available position both on grid and off grid</summary>
        public GridInstance GetNearestPosition(Vector3 position, GameObject gameObj, BoardItem item, GridInstance previous = null)
        {
            Vector3? posToIgnore = null;
            bool isBottomRow = false;
            if (previous != null)
            {
                isBottomRow = previous.index < this.generator.Width;
                posToIgnore = previous.position;
            }
            
            if (!isBottomRow)
                return GetNearestGridPosition(position, gameObj, item, posToIgnore);
            return GetNearestOffGridPosition(position, gameObj, item, posToIgnore, previous.offGrid);
        }

        /// <summary>Used in case the grid is full, a last row underneath the board. Try to get a available position</summary>
        public GridInstance GetNearestOffGridPosition(Vector3 position, GameObject gameObj, BoardItem item,
            Vector3? posToIgnore = null, bool isAlreadyOffGrid = false)
        {
            int foundIndexOffGrid = GetNearestIndex(position, this.availableOffGridPositions, posToIgnore);
            int foundIndex = GetNearestIndex(position, this.availablePositions, posToIgnore);
            
            if (foundIndexOffGrid == -1 && !isAlreadyOffGrid)
                return RegisterGridPosition(gameObj, item, foundIndex);
            if (isAlreadyOffGrid)
                return null;
            
            GridInstance newInstance = new(gameObj, foundIndexOffGrid, this.generator, true, item, item?.GetBoardComponent<GridMoveable>());
            return RegisterInGrid(newInstance);
        }

        /// <summary>Tries to find the nearest available position on the board</summary>
        public GridInstance GetNearestGridPosition(Vector3 position, GameObject gameObj, BoardItem item,
            Vector3? posToIgnore = null)
        {
            int foundIndex = GetNearestIndex(position, this.availablePositions, posToIgnore);
            return foundIndex == -1 ? null : RegisterGridPosition(gameObj, item, foundIndex);
        }

        /// <summary>
        /// Creates a grid instance, and makes sure the position isn't taken again
        /// </summary>
        private GridInstance RegisterGridPosition(GameObject gameObj, BoardItem item, int foundIndex) => 
            RegisterInGrid(new GridInstance(gameObj, foundIndex, this.generator, false, item, item?.GetBoardComponent<GridMoveable>()));

        /// <summary>
        /// Registers a grid instance, making sure to occupy the space inside the grid.
        /// </summary>
        /// <param name="instance">instance to register</param>
        /// <returns>the instance at the desired location, will return already existing obj if place already taken</returns>
        public GridInstance RegisterInGrid(GridInstance instance)
        {
            GridInstance found =
                this.gridInstances.FirstOrDefault(a => a.index == instance.index && a.offGrid == instance.offGrid);
            if (found != null) return found;
            
            if (!instance.offGrid)
                this.availablePositions[instance.index] = null;
            else
                this.availableOffGridPositions[instance.index] = null;
            
            this.gridInstances.Add(instance);
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

        /// <summary>
        /// A method to call a method on all taken grid objects
        /// </summary>
        /// <param name="action">to invoke on all taken positions</param>
        public void ForAllGridItems(Action<GridInstance> action)
        {
            List<GridInstance> tempGridInstances = new(this.gridInstances);
            tempGridInstances.Sort((a, b) => a.position.y.CompareTo(b.position.y));
            tempGridInstances.ForEach(action);
        }
    }
}