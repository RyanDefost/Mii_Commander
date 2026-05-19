using System.Collections.Generic;

namespace Managers
{
    public class FiniteStateMachine<T>
    {
        private T owner;
        private State<T> currentState;

        public FiniteStateMachine(T owner , State<T> currentState)
        {
            this.owner = owner;
            
            currentState.Owner = owner;
            SetState(currentState);
        }
        
        public void Update()
        {
            if (this.currentState != null )
            {
                this.currentState.Update();
            }
        }

        public void SetState( State<T> newState )
        {
            this.currentState?.Exit();
            
            newState.Owner = this.owner;
            newState.Start();

            this.currentState = newState;
        }
    }
}