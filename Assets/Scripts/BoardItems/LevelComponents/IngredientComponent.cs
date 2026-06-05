using System.Collections.Generic;
using UnityEngine;

namespace BoardItems
{
    public class IngredientComponent : BoardItemComponent
    {
        [SerializeField] private List<GameObject> ingredients = new();
        
        public List<GameObject> GetIngredients() => this.ingredients;
    }
}