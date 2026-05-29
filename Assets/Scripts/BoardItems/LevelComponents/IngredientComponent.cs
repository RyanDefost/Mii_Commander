using System.Collections.Generic;
using UnityEngine;

namespace Synergy
{
    public class IngredientComponent : BoardItemComponent
    {
        [SerializeField] private List<GameObject> ingredients = new();
        
        public List<GameObject> GetIngredients() => ingredients;
    }
}