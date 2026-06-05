namespace StateMachine
{
    [System.Serializable]
    public abstract class State<T>
    {
        public T Owner { get; set; }

        protected State(T owner)
        {
            this.Owner = owner;
        }
        
        public abstract void Start();
        public abstract void Update();
        public abstract void Exit();
    }
}