using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera camRef;

    private void Awake() => this.camRef = Camera.main;
    void Update() => this.transform.LookAt(this.camRef.transform.position, -Vector3.up);
}
