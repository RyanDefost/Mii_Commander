using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class LevelChanger : MonoBehaviour
    {
        public void ChangeLevel(string levelName)
        {
            SceneManager.LoadSceneAsync(levelName);
        }

        public void Quit()
        {
            Debug.Log("Quitting game...");
            Application.Quit();
        }
    }
}