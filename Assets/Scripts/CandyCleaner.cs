using System.Collections;
using System.Collections.Generic;
using BoardItems;
using Grid;
using UnityEngine;
using Random = UnityEngine.Random;

public class CandyCleaner : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private Vector2 pushSpeedRangeY = new(-7, -10);
    [SerializeField] private Vector2 pushSpeedRangeZ = new(-3, -5);
    [Space, SerializeField] private Vector2 pushBufferRange = new Vector2(0.01f, 0.04f);

    private readonly HashSet<BoardItem> destroyRequested =  new();

    [Space, SerializeField] private Animator animator;
    [SerializeField] private AnimationClip playWind;
    
    private void Awake() => ComponentRegistry.AddToRegistry(this);

    public void CleanBoard() => StartCoroutine(PushCandy());

    private IEnumerator PushCandy()
    {
        GridMoveable[] moveables = FindObjectsByType<GridMoveable>(FindObjectsSortMode.None);
        
        foreach (GridMoveable moveable in moveables)
        {
            BoardItem boardItem = moveable.GetBoardItem();
            if (boardItem.gridInstanceRef != null || this.destroyRequested.Contains(boardItem) || moveable.skipCleaning) continue;

            this.animator.Play(this.playWind.name);
            moveable.ApplyImpulse(new Vector3(
                0,
                Random.Range(this.pushSpeedRangeY.x, this.pushSpeedRangeY.y),
                Random.Range(this.pushSpeedRangeZ.y, this.pushSpeedRangeZ.y))
            );
                
            StartCoroutine(SetForDestroy(boardItem, 5f));
            yield return new WaitForSeconds(Random.Range(this.pushBufferRange.x, this.pushBufferRange.y));
        }
    }

    private IEnumerator SetForDestroy(BoardItem boardItem, float time)
    {
        this.destroyRequested.Add(boardItem);
        yield return new WaitForSeconds(time);
        this.destroyRequested.Remove(boardItem);
        
        if (boardItem == null) yield break;
        if (boardItem.gridInstanceRef == null)
            DestroyImmediate(boardItem.gameObject);
            
    }
}