using System.Collections;
using UnityEngine;

namespace HelperStructs
{
    public class CandyCleaner : MonoBehaviour
    {
        public void CleanBoard()
        {
            StartCoroutine(PushCandy());
        }

        private IEnumerator PushCandy()
        {
            GridMoveable[] moveables = Object.FindObjectsByType<GridMoveable>(FindObjectsSortMode.None);
            foreach (GridMoveable moveable in moveables)
            {
                if (moveable.GetBoardItem().gridInstanceRef != null) continue;

                int randY = Random.Range(-7, -10);
                int randZ = Random.Range(-3, -5);
                float wait = Random.Range(0.01f, 0.04f);
                
                moveable.ApplyImpulse(new Vector3(0, randY, randZ));
                Destroy(moveable.gameObject, 5f);
                
                yield return new WaitForSeconds(wait);
            }
            
            
            
        }
    }
}