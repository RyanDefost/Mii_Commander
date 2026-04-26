using System.Collections.Generic;
using UnityEngine;

public class AreaSelector : MonoBehaviour
{
    [SerializeField] private int maxTargets = 10;
    [SerializeField] private DetectableTypes type = DetectableTypes.GOOBER;
    
    private readonly List<IDetectable> pickables = new();
    
    public List<IDetectable> GetSelection(Vector3 detectPoint,float areaRange, DetectableTypes detectableType)
    {
        pickables.Clear();
        
        Collider[] hitColliders = new Collider[this.maxTargets];
        int amount = Physics.OverlapSphereNonAlloc(detectPoint, areaRange, hitColliders);
        for (int i = 0; i < amount; i++)
        {
            hitColliders[i].TryGetComponent(out IDetectable detectable);
            if (detectable == null) continue;
            
            if (detectable.DetectableType == detectableType)
                this.pickables.Add(detectable);
        }
        
        return this.pickables; 
    }
    
    public List<IDetectable> GetSelection(Vector3 detectPoint,float areaRange, DetectableTypes[] detectableTypes)
    {
        pickables.Clear();
        
        Collider[] hitColliders = new Collider[this.maxTargets];
        int amount = Physics.OverlapSphereNonAlloc(detectPoint, areaRange, hitColliders);
        for (int i = 0; i < amount; i++)
        {
            hitColliders[i].TryGetComponent(out IDetectable detectable);
            if (detectable == null) continue;

            foreach (DetectableTypes detectType in detectableTypes)
            {
                if(detectable.DetectableType == detectType)
                    this.pickables.Add(detectable);
            }
        }
        
        return this.pickables; 
    }
}
