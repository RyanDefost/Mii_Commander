using System.Linq;
using Grid;
using Managers;
using UnityEngine;

namespace Synergy
{
    public class InteractionVisualizer : BoardItemComponent
    {
        [SerializeField] private GameObject visualizer;
        [SerializeField] private float heightOffset = 0f;

        private SynergyLookUp synergyLookUp;
        private GridManager gridManager;
        
        private GridRotatable gridRotatable;
        private GridMoveable gridMoveable;

        private Vector3 lastParentPosition;
        
        public override void ConnectToBoardItem()
        {
            visualizer.transform.SetParent(null);
            visualizer.SetActive(false);
            
            this.gridRotatable = this.GetComponent<GridRotatable>();
            this.gridMoveable = this.GetComponent<GridMoveable>();
            
            this.gridMoveable.OnMoved += UnparentVisual;
            
            this.boardItem.OnAddToHand += HideVisual;
        }
        
        private void OnDestroy() => Destroy(visualizer);
        
        /// <summary>
        /// Gets visual position and activates visual.
        /// </summary>
        private void ActivateVisual()
        {
            //Init
            this.synergyLookUp ??= ComponentRegistry.GetComponent<SynergyLookUp>();
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            
            //Get Neighbors.
            NeighborPattern? sidePattern = GetForwardNeighborPattern(this.transform.rotation.eulerAngles.z);
            GridManager.GridInstance[] foundNeighbors = this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, sidePattern.Value);
            GridManager.GridInstance newTarget = foundNeighbors.FirstOrDefault(neighbor => neighbor != null);

            //Else Get empty neighbor.
            if (newTarget == null)
            {
                newTarget = this.gridManager.GetNearestPosition(
                    this.boardItem.gridInstanceRef.position + (this.gridManager.CellSize.y) * this.gridRotatable.StaticForwardDirection,  
                    this.boardItem.gridInstanceRef.gameObj, this.boardItem, this.boardItem.gridInstanceRef); 
                this.gridManager.ReleaseInstance(newTarget);
            }
            
            //Set visual transform.
            this.visualizer.transform.rotation = Quaternion.Euler(Vector3.zero);
            this.visualizer.transform.position = newTarget.position + new Vector3(0,0,heightOffset);

            this.visualizer.SetActive(true);
            
            this.boardItem.OnAddToHand += HideVisual;
            this.boardItem.OnAddToBoard -= ActivateVisual;
        }

        /// <summary>
        /// Hides visual and adds activation trigger.
        /// </summary>
        private void HideVisual()
        {
            this.visualizer.SetActive(false);
            
            this.lastParentPosition = Vector3.zero;
            this.gridMoveable.OnStartMoving -= ParentVisual;
            
            this.boardItem.OnAddToHand -= HideVisual;
            this.boardItem.OnAddToBoard += ActivateVisual;   
        }

        private void ParentVisual()
        {
            this.lastParentPosition = this.transform.position;
        }

        private void UnparentVisual()
        {
            if (this.lastParentPosition == Vector3.zero)
            {
                this.gridMoveable.OnStartMoving += ParentVisual;
                return;
            }
            
            Vector3 direction = this.transform.position - this.lastParentPosition;
            this.visualizer.transform.position += new Vector3(direction.x, direction.y, 0);   
        } 
        
        /// <summary>
        /// Helper function to convert eulerAngles.Z into pattern direction.
        /// </summary>
        /// <param name="rotation">Object eulerAngles.Z</param>
        /// <returns>Returns corresponding NeighborPattern</returns>
        private static NeighborPattern GetForwardNeighborPattern(float rotation)
        {
            int[] directions = { 0, 90, 180, 270 };
            int nearest = directions.OrderBy(x => Mathf.Abs((long) x - rotation)).First();
            return nearest switch
            {
                0 => NeighborPattern.Up,
                90 => NeighborPattern.Left,
                180 => NeighborPattern.Down,
                270 => NeighborPattern.Right,
                var _ => NeighborPattern.Up
            };
        }
    }
}