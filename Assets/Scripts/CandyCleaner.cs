using System;
using System.Collections;
using System.Collections.Generic;
using Scoring;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HelperStructs
{
    public class CandyCleaner : MonoBehaviour
    {
        [Header("Push Settings")]
        [SerializeField] private Vector2 pushSpeedRangeY = new(-7, -10);
        [SerializeField] private Vector2 pushSpeedRangeZ = new(-3, -5);
        [Space, SerializeField] private Vector2 pushBufferRange = new Vector2(0.01f, 0.04f);

        private void Awake() => ComponentRegistry.AddToRegistry(this);

        public void CleanBoard() => StartCoroutine(PushCandy());

        private IEnumerator PushCandy()
        {
            GridMoveable[] moveables = FindObjectsByType<GridMoveable>(FindObjectsSortMode.None); //TODO: ALSO GRABS ITEMS THAT WILL BE DESTROYED!
            foreach (GridMoveable moveable in moveables)
            {
                BoardItem boardItem = moveable.GetBoardItem();
                if (boardItem.gridInstanceRef != null) continue;
                
                moveable.ApplyImpulse(new Vector3(
                    0,
                    Random.Range(pushSpeedRangeY.x, pushSpeedRangeY.y),
                    Random.Range(pushSpeedRangeZ.y, pushSpeedRangeZ.y))
                );
                
                StartCoroutine(SetForDestroy(boardItem, 5f));
                yield return new WaitForSeconds(Random.Range(pushBufferRange.x, pushBufferRange.y));
            }
        }

        private static IEnumerator SetForDestroy(BoardItem boardItem, float time)
        {
            yield return new WaitForSeconds(time);
            
            if(boardItem.gameObject || boardItem.gridInstanceRef == null)
                DestroyImmediate(boardItem.gameObject);
        }
    }
}