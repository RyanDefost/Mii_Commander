using Grid;
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
        
        //[SerializeField] private ScoreManager scoreManager;
        //[SerializeField] private QuestManager questManager;
        
        [Header("Game Settings")]
        [SerializeField] private LevelRef levelData;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            this.moveManager = FindFirstObjectByType<MoveManager>();
            this.turnManager = FindFirstObjectByType<TurnManager>();
            //this.questManager = FindFirstObjectByType<QuestManager>();
        }

        private void OnEnable()
        {
            ComponentRegistry.AddToRegistry(this);
            InitGame();
        }
        
        private void OnDisable() => ComponentRegistry.RemoveFromRegistry(this);

        private void InitGame()
        {
            this.moveManager.InitialMoveAmount = levelData.baseTurnAmount;
        }
    }
}
