using System.Collections.Generic;

namespace Managers
{
    public class FiniteStateMachine<T>
    {
        private T owner;
        public State<T> CurrentState { get; private set; }

        public FiniteStateMachine(T owner , State<T> currentState)
        {
            this.owner = owner;
            
            currentState.Owner = owner;
            SetState(currentState);
        }
        
        public void Update()
        {
            if ( CurrentState != null )
            {
                CurrentState.Update();
            }
        }

        public void SetState( State<T> newState )
        {
            CurrentState?.Exit();
            
            newState.Owner = owner;
            newState.Start();
            
            CurrentState = newState;
        }
    }
}