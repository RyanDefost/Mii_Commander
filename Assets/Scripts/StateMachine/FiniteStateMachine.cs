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
            if ( currentState != null )
            {
                currentState.Update();
            }
        }

        public void SetState( State<T> newState )
        {
            currentState?.Exit();
            
            newState.Owner = owner;
            newState.Start();
            
            currentState = newState;
        }
    }
}