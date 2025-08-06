using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

namespace Rooms
{
    [CreateAssetMenu(menuName = "World/World Config", fileName = "WorldConfig")]
    public class WorldConfig : ScriptableObject
    {
        [Header("Room Prefabs")]
        public List<GameObject> pantryLayouts;
        public List<GameObject> freezerLayouts;
        public List<GameObject> gardenLayouts;
        
        [Header("Generation Settings")]
        public float roomSpacing = 20f;
        
        [Header("Recipe list")]
        public List<ItemDefinition> recipeItems;
        public List<ItemDefinition> trashItems;
    }
}