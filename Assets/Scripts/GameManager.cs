using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField, Header("Input Actions")]
    private InputActionAsset inputActionAsset;
    private InputActionMap inputActionMap;
    private InputAction resetAction;
    
    private void Awake() => InitControls();
    
    private void InitControls()
    {
        this.inputActionMap = this.inputActionAsset.FindActionMap("UI");
        this.inputActionMap.Enable();

        this.resetAction = this.inputActionMap.FindAction("Reset");
        this.resetAction.performed += context => Reset();
    }

    private void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
