namespace StateMachine
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
            if (this.CurrentState != null )
            {
                this.CurrentState.Update();
            }
        }

        public State<T> SetState( State<T> newState )
        {
            this.CurrentState?.Exit();
            
            newState.Owner = this.owner;
            newState.Start();

            this.CurrentState = newState;
            return this.CurrentState;
        }
    }
}