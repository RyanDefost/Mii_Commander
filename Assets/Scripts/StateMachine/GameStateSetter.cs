using UnityEngine;

namespace Managers
{
    public class GameStateSetter : MonoBehaviour
    {
        [SerializeField] private GameState state;
        private GameManager gameManager;
        
        public void SetState()
        {
            this.gameManager = ComponentRegistry.GetComponent<GameManager>();
            this.gameManager.SetGameState(this.state);
        }
    }
}