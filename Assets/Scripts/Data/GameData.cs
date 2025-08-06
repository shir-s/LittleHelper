using System.Collections.Generic;
using Rooms;
using Scriptable_Objects;
using UnityEngine;

namespace Data
{
    public class GameData
    {
        [SerializeField] private int rounds = 7;
        [SerializeField] private List<ItemDefinition> recipeItems;
        [SerializeField] private List<ItemDefinition> trashItems;
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] internal GameObject playerObject;
        
        
        
    }
}