using System;
using System.Collections.Generic;
using GameData;
using Grid;
using Managers.GameStates;
using Managers.Quest;
using Scoring;
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
        public ScoreManager ScoreManager { get => this.scoreManager; private set => this.scoreManager = value; }
        
        [SerializeField]
        private QuestManager questManager;
        public QuestManager QuestManager { get => this.questManager; private set => this.questManager = value; }
        
        //[SerializeField] private QuestManager questManager;
        
        public Action<GameState> OnStateChange;
        
        [Header("Game Settings")]
        [SerializeField] private LevelRef levelData;
        
        private FiniteStateMachine<GameManager> gameStateMachine;
        private readonly Dictionary<GameState, State<GameManager>> states = new();

        public Action<GameState> OnChangeState;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            this.moveManager = FindFirstObjectByType<MoveManager>();
            this.turnManager = FindFirstObjectByType<TurnManager>();
            this.scoreManager = FindFirstObjectByType<ScoreManager>();
            this.questManager = FindFirstObjectByType<QuestManager>();
        }

        private void Awake() => ComponentRegistry.AddToRegistry(this);

        private void Start()
        {
            InitGame();
            InitStates();
        }

        private void OnDestroy() => ComponentRegistry.RemoveFromRegistry(this);
        private void Update() => this.gameStateMachine.Update();

        private void InitGame()
        {
            this.questManager.SetQuests(this.levelData.GetQuests());
            
            this.gameStateMachine = new FiniteStateMachine<GameManager>(this, new PlayGameState(this));
            this.OnStateChange?.Invoke(GameState.PLAY);
            
            this.moveManager.SetInitialMoveAmount(this.levelData.baseTurnAmount);
            this.scoreManager.SetGoal(this.levelData.pointRequirement);
        }

        private void InitStates()
        {
            this.states.Add(GameState.PLAY, new PlayGameState(this));
            this.states.Add(GameState.CANDYMOVE, new CandyMoveState(this));
            this.states.Add(GameState.CANDYACTIVATE, new CandyActivateState(this));
        }

        public void SetGameState(GameState stateKey)
        {
            if (this.states.TryGetValue(stateKey, out State<GameManager> state))
            {
                if (state == this.gameStateMachine.CurrentState) return;
                this.gameStateMachine.SetState(state);   
                this.OnStateChange?.Invoke(stateKey);
            }
        }
    }
}
