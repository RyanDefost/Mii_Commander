using System;
using System.Collections.Generic;
using BoardItems;
using Grid;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(GridGenerator), typeof(GridManager))]
public class InitialItemSpawner : MonoBehaviour
{
    [SerializeField]
    private GridGenerator gridGenerator;
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private InitialItem[] items;
    
    [Serializable]
    public class InitialItem
    {
        public int index;
        public GameObject prefab;
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        this.gridGenerator = GetComponent<GridGenerator>();
        this.gridManager = GetComponent<GridManager>();
        
        if (!this.gridGenerator || !this.gridManager) return;

        List<Vector3?> positions = this.gridGenerator.GetAllPositions();
        foreach (InitialItem item in this.items) item.index = math.clamp(item.index, 0, positions.Count - 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (!this.gridGenerator || !this.gridManager) return;

        Gizmos.color = Color.red;
        List<Vector3?> positions = this.gridGenerator.GetAllPositions();
        foreach (InitialItem item in this.items)
        {
            Vector3? pos =  positions[item.index];
            if (pos == null) continue;
            Gizmos.DrawSphere(pos.Value, 0.1f);
        }
    }

    private void Awake()
    {
        if (!this.gridGenerator || !this.gridManager) return; 
        this.gridGenerator.generationIsDone += OnGenerationIsDone;
    }

    private void OnDestroy()
    {
        if (!this.gridGenerator || !this.gridManager) return; 
        this.gridGenerator.generationIsDone -= OnGenerationIsDone;
    }

    private void OnGenerationIsDone()
    {
        List<Vector3?> positions = this.gridGenerator.GetAllPositions();
        foreach (InitialItem item in this.items)
        {
            if (item.index == -1 || !item.prefab) continue;
            Vector3? spawnPos = positions[item.index];
            if (spawnPos == null) continue;
            GameObject spawnedObject = Instantiate(item.prefab, spawnPos.Value + Vector3.back, this.transform.rotation);
            BoardItem boardItem = spawnedObject.GetComponent<BoardItem>();
            GridMoveable moveComponent = spawnedObject.GetComponent<GridMoveable>();

            boardItem.OnStarted += OnBoardItemOnStarted;
            continue;

            void OnBoardItemOnStarted(BoardItem _)
            {
                boardItem.OnStarted -= OnBoardItemOnStarted;
                GridManager.GridInstance newTarget = this.gridManager.GetNearestPosition(spawnPos.Value, spawnedObject, boardItem);
                if (newTarget == null) return;

                moveComponent.SetTarget(newTarget, false);
                moveComponent.SetMoving(true);
                
                if (spawnedObject.TryGetComponent(out CountDownTillActivation countDownTillActivation))
                    countDownTillActivation.EnableActivate();
            }
        }
    }
}
