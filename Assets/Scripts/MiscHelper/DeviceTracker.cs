using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Device tracker, checks if player is using keyboard or gamepad
/// </summary>
public class DeviceTracker : MonoBehaviour
{
    public InputDevice LastDevice {get; private set;}
    
    private void Start() => ComponentRegistry.AddToRegistry(this);

    // Subscribe to the global input event
    private void OnEnable() => InputSystem.onActionChange += OnActionChange;

    // Unsubscribe to prevent memory leaks
    private void OnDisable() => InputSystem.onActionChange -= OnActionChange;

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change is not (InputActionChange.ActionStarted or InputActionChange.ActionPerformed)) return;
        InputAction action = (InputAction)obj;
        InputDevice currentDevice = action.activeControl.device;
        
        if (currentDevice == this.LastDevice)
            return;
        this.LastDevice = currentDevice;

        // switch (currentDevice) //Could be useful, leave for now, TODO remove later
        // {
        //     case Gamepad:
        //         Debug.Log("Switched to Gamepad!");
        //         break;
        //     case Mouse or Keyboard:
        //         Debug.Log("Switched to Keyboard/Mouse!");
        //         break;
        // }
    }
}