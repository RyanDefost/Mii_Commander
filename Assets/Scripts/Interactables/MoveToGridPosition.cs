using System;
using Grid;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveToGridPosition : MonoBehaviour
{
    [SerializeField]
    private float offset = 0.25f;
    private GridManager grid;
    [SerializeField] 
    private Rigidbody rb;
    [SerializeField]
    private float movementStrength = 5f;
    [SerializeField]
    private float maxVelocity = 20f;
    [SerializeField]
    private float dampening = 2f;
    [SerializeField]
    private float startTimerTime;
    private Timer startTimer;
    [SerializeField]
    private float endTimerTime = 15f;
    private Timer endTimer;
    private Action onUpdate;
    private GridManager.GridInstance target;
    private float usedMinimumForce;

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        this.rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.grid = ComponentRegistry.GetComponent<GridManager>();
        SetEnabled(true);
    }

    public void SetEnabled(bool newState)
    {
        if (newState)
            DoEnable();
        else
            DoDisable();
    }
    
    private void DoEnable()
    {
        this.startTimer = new Timer(this.startTimerTime, StartMovement);
        this.onUpdate += UpdateStartTimer;
    }

    private void DoDisable()
    {
        this.onUpdate -= UpdateStartTimer;
        this.onUpdate -= ApplyForceTowardsTarget;
        this.onUpdate -= UpdateEndTimer;
        ResetTarget();
    }

    private void Update() => this.onUpdate?.Invoke();
    private void UpdateStartTimer() => this.startTimer.Update(Time.deltaTime);

    private void StartMovement()
    {
        if (!GetTarget())
            return;
        this.usedMinimumForce = this.movementStrength;
        this.endTimer = new Timer(this.endTimerTime, SnapToTarget);
        
        this.onUpdate += ApplyForceTowardsTarget;
        this.onUpdate += UpdateEndTimer;
        this.onUpdate -= UpdateStartTimer;
        
        this.startTimer.Reset();
    }
    
    private bool GetTarget()
    {
        if (this.target != null)
            return true;
        GridManager grid = ComponentRegistry.GetComponent<GridManager>();
        SetTarget(grid.GetNearestPosition(this.transform.position, this.gameObject));
        return this.target != null;
    }
    
    public void SetTarget(GridManager.GridInstance newTarget) => this.target = newTarget;

    private void UpdateEndTimer() => this.endTimer.Update(Time.deltaTime);

    private Vector3 GetTargetPosition() => this.target.position + Vector3.back * this.offset;

    private void SnapToTarget()
    {
        if (this.target == null)
            return;
        this.transform.position = GetTargetPosition();
        this.rb.linearVelocity = Vector3.zero;
        this.rb.angularVelocity = Vector3.zero;
        this.rb.Sleep();
        this.onUpdate -= UpdateEndTimer;
        
        this.endTimer.Reset();
    }
    
    private void ApplyForceTowardsTarget()
    {
        if (this.target == null)
            return;
        Vector3 targetPosition = GetTargetPosition();
        Vector3 diff = targetPosition - this.transform.position;

        Vector3 velocity = this.rb.linearVelocity;
        float dot = velocity.sqrMagnitude > 0.001f
            ? Vector3.Dot(diff.normalized, velocity.normalized)
            : 0f;

        // Reduce force when already moving toward target
        float alignmentFactor = 1f - Mathf.Clamp01(dot); 
        // dot = 1 → factor = 0 (no extra push)
        // dot = 0 → factor = 1 (normal)
        // dot = -1 → factor = 2 (strong correction)

        float force = Mathf.Max(diff.magnitude, this.usedMinimumForce) * alignmentFactor;
        this.rb.AddForce(diff.normalized * force, ForceMode.Force);
        
        if (this.rb.linearVelocity.magnitude > this.maxVelocity)
            this.rb.linearVelocity = this.rb.linearVelocity.normalized * this.maxVelocity;

        if (!(diff.magnitude < 0.2f)) return;
        this.rb.linearVelocity *= 1f - Time.deltaTime * this.dampening;
        this.usedMinimumForce = Mathf.Max(
            this.usedMinimumForce - Time.deltaTime * this.dampening,
            0f
        );
        if (this.rb.linearVelocity.magnitude < 0.2f)
            SnapToTarget();
    }

    public void ResetTarget()
    {
        if (this.target == null)
            return;
        this.grid.ReleaseInstance(this.target);
        this.target = null;
    }

    public bool HasTarget() => this.target != null;
}
