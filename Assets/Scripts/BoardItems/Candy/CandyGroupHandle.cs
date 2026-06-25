using System.Collections.Generic;
using PlayerHand;
using UnityEngine;

namespace BoardItems
{
    /// <summary>
    /// A class for managing multiple candy instances,
    /// this would make it more efficient to reuse dead candy when adding multiple to the hand
    /// TODO, actually reuse groups
    /// </summary>
    public class CandyGroupHandle : IPoolable
    {
        public bool Active { get; set; }

        private readonly List<CandyHandle> instances;
        private readonly PlayerHandManager playerHandRef;

        public CandyGroupHandle(PlayerHandManager playerHandManager, PlayerDeck.DeckOption deck, Transform parent, int amount)
        {
            const float clumpingDist = 0.3f;
        
            this.instances = new List<CandyHandle>();
            for (int i = 0; i < amount; i++)
            {
                deck.GetCandy(out GameObject prefab, out int typeIndex);
            
                GameObject gameObject = Object.Instantiate(prefab, parent, false);
                
                Vector3 parentScale = parent.localScale;
                Vector3 prefabScale = prefab.transform.localScale;

                gameObject.transform.localScale = new Vector3(
                    prefabScale.x / parentScale.x,
                    prefabScale.y / parentScale.y,
                    prefabScale.z / parentScale.z
                );
                
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.position += new Vector3(Random.Range(-clumpingDist, clumpingDist), Random.Range(-clumpingDist, clumpingDist), 0);

                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezePosition;
            
                CandyActor actor = gameObject.GetComponent<CandyActor>();
                actor.candyType = typeIndex;
            
                this.instances.Add(new CandyHandle(rb, actor, gameObject));
            }

            this.playerHandRef = playerHandManager;
            playerHandManager.OnRemovedFromHand += OnGrabReleased;
        }

        public void OnEnableObject()
        {
            foreach (CandyHandle instance in this.instances) instance.gameObject.SetActive(true);
        }

        public void OnDisableObject()
        {
            foreach (CandyHandle instance in this.instances)
            {
                instance.gameObject.SetActive(false);
                instance.rb.constraints = RigidbodyConstraints.FreezePosition;
            }
        }

        private void OnGrabReleased(Vector2 handMovementDir, Vector2 throwForce)
        {
            foreach (CandyHandle instance in this.instances) 
                instance.actor.OnGrabReleased(instance.rb, handMovementDir, throwForce);
            this.playerHandRef.OnRemovedFromHand -= OnGrabReleased;
        }

        public void SetParent(Transform parent)
        {
            foreach (CandyHandle instance in this.instances)
            {
                instance.gameObject.transform.parent = parent;
                instance.gameObject.transform.localPosition = Vector3.zero;
                instance.gameObject.transform.position += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
            }
        }
    }
}