using UnityEngine;

/// <summary>
/// A simple component, that makes the transform follow the camera
/// </summary>
public class Billboard : MonoBehaviour
{
    private Camera camRef;

    private void Awake() => this.camRef = Camera.main;
    private void LateUpdate() => this.transform.LookAt(this.camRef.transform.position, -Vector3.up);
}
