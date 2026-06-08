using UnityEngine;

namespace BoardItems
{
    public class ShakeDetector : BoardItemComponent
    {
        [SerializeField] private BoardItemComponent toActivate;
        [SerializeField] private float shakeDistanceDifference = 1f;
        [SerializeField,  Range(-0.4f, 0)] private float shakeDirectionDifference = -0.2f;
        [SerializeField] private int shakeThreshold = 3;
        private Vector3? lastPosition;
        private Vector3 lastDir;
        private int shakeCount;
        private IBoardItemDisablable currentToActivate;
        private bool isBeingHeld;

        protected override void CustomOnValidate()
        {
            base.CustomOnValidate();
            this.enabled = this.toActivate;
            if (!this.toActivate || this.toActivate is IBoardItemDisablable) return;
            this.toActivate = null;
            Debug.LogError($"{GetType()}-{GetInstanceID()}: toActivate needs to be {nameof(IBoardItemDisablable)}");
        }
    
        private void Awake()
        {
            this.currentToActivate = (IBoardItemDisablable)this.toActivate;
            this.currentToActivate.DisableActivate();
            this.isBeingHeld = false;
        }

        public override void ConnectToBoardItem()
        {
            this.boardItem.OnAddToHand += OnAddToHand;
            this.boardItem.OnRemovedFromHand += OnRemovedFromHand;
        }

        private void OnRemovedFromHand()
        {
            this.isBeingHeld = false;
            this.lastPosition = null;
            this.lastDir = Vector3.zero;
            this.shakeCount = 0;
            this.currentToActivate.EnableActivate();
        }

        private void OnAddToHand()
        {
            this.shakeCount = 0;
            this.lastPosition = null;
        
            this.isBeingHeld = true;
            this.currentToActivate.DisableActivate();
        }

        private void Update()
        {
            if (!this.isBeingHeld)
                return;

            if (this.lastPosition.HasValue)
            {
                Vector3 diff = this.lastPosition.Value - this.transform.position;
                Vector3 dir  = diff.normalized;

                if (diff.magnitude >= this.shakeDistanceDifference &&
                    Vector3.Dot(dir, this.lastDir) < this.shakeDirectionDifference) 
                    this.shakeCount++;

                this.lastPosition = Vector3.Lerp(this.lastPosition.Value, this.transform.position, Time.deltaTime * 10f);
                this.lastDir = Vector3.Lerp(this.lastDir, dir, Time.deltaTime * 10f);
            }
            else
            {
                this.lastPosition = this.transform.position;
                this.lastDir = Vector3.zero;
            }

            if (this.shakeCount < this.shakeThreshold) return;
            this.shakeCount = 0;
            this.currentToActivate.Activate();
        }
    }
}