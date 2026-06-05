using System;
using UnityEngine;

namespace BoardItems
{
    /// <summary>
    /// Manages the in game logic of a piece of candy
    /// </summary>
    [RequireComponent(typeof(MoveToGridPosition), typeof(HandHandler))]
    public class CandyActor : BoardItem, IUserInterfaceValueGetter
    {
        [SerializeField] private int points;
        public int Points { get => this.points; private set => this.points = value; }

        public int candyType; // index reference to lookup

        private Action<object, Type> OnchangePoints;
    
        [SerializeField]
        private MoveToGridPosition movement;
        [SerializeField]
        private HandHandler handHandler;
    
        protected virtual void CustomOnValidate()
        {
            this.movement = GetComponent<MoveToGridPosition>();
            this.handHandler = GetComponent<HandHandler>();
        }

        public void OnGrabReleased(Rigidbody rb, Vector2 handMovementDir, Vector2 throwForce)
        {
            this.movement.moveImmunity = true;
            HandHandler.OnGrabReleased(rb, this, handMovementDir, throwForce);
            this.OnAddToBoard += RemovePlayerMoveImmunity;
        }

        private void RemovePlayerMoveImmunity()
        {
            this.movement.moveImmunity = false;
            this.OnAddToBoard -= RemovePlayerMoveImmunity;
        }

        public void ApplyPointMultiplier(float multiplier)
        {
            this.points = Mathf.CeilToInt(this.points * multiplier);   
            this.OnchangePoints?.Invoke(this.points, typeof(int));
        }
    
        public object GetValue(string valueName, out Type returnType)
        {
            if (valueName == "points")
            {
                returnType = typeof(int);
                return this.Points;
            }

            returnType = null;
            return null;
        }

        public void SubscribeOnChangeValue(string valueName, Action<object, Type> callback)
        {
            if (valueName == "points") this.OnchangePoints += callback;
        }

        public void UnSubscribeOnChangeValue(string valueName, Action<object, Type> callback)
        {
            if (valueName == "points") this.OnchangePoints -= callback;
        }
    }
}