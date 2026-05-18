using System;
using System.Collections.Generic;
using Grid;
using Managers.GameStates;
using Managers.Scoring;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private MoveManager moveManager;
        public MoveManager MoveManager { get => this.moveManager; private set => this.moveManager = value; }
        
        [SerializeField] private TurnManager turnManager;
        public TurnManager TurnManager { get => this.turnManager; private set => this.turnManager = value; }
        
        [SerializeField] private ScoreManager scoreManager;
        public  ScoreManager ScoreManager { get => this.scoreManager; private set => this.scoreManager = value; }
        
        //[SerializeField] private QuestManager questManager;
        
        [Header("Game Settings")]
        [SerializeField] private LevelRef levelData;
        
        private FiniteStateMachine<GameManager> gameStateMachine;
        private readonly Dictionary<GameState, State<GameManager>> states = new();

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            this.moveManager = FindFirstObjectByType<MoveManager>();
            this.turnManager = FindFirstObjectByType<TurnManager>();
            this.scoreManager = FindFirstObjectByType<ScoreManager>();
            //this.questManager = FindFirstObjectByType<QuestManager>();
        }

        private void Awake() => ComponentRegistry.AddToRegistry(this);

        private void Start()
        {
            InitStates();
            InitGame();
        }

        private void OnDestroy() => ComponentRegistry.RemoveFromRegistry(this);

        private void Update()
        {
            this.gameStateMachine.Update();
        }

        private void InitGame()
        {
            this.gameStateMachine = new FiniteStateMachine<GameManager>(this, new PlayGameState());
            
            this.moveManager.SetInitialMoveAmount(levelData.baseTurnAmount);
        }

        private void InitStates()
        {
            this.states.Add(GameState.PLAY, new PlayGameState());
            this.states.Add(GameState.WAIT, new WaitGameState());
        }

        public void SetGameState(GameState stateKey)
        {
            if (states.TryGetValue(stateKey, out State<GameManager> state))
            {
                this.gameStateMachine.SetState(state);   
            }
        }
    }
}
