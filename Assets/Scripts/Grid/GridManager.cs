using System;
using UnityEngine;

namespace Grid
{
    [RequireComponent(typeof(GridGenerator))]
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private GridGenerator generator;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.generator = GetComponent<GridGenerator>();
        }
    }
}