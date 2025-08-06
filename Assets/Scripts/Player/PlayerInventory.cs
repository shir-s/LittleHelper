using Scriptable_Objects;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        public ItemDefinition CurrentItem { get; private set; }
        
        private void OnEnable()
        {
            GameEvents.RestartLevel += HandleRestart;
        }

        private void OnDisable()
        {
            GameEvents.RestartLevel -= HandleRestart;
        }

        private void HandleRestart()
        {
            DropItem();
        }
        
        public bool PickUp(ItemDefinition item)
        {
            if(CurrentItem != null) return false;
            CurrentItem = item;
            // TODO: update UI 
            print($"Picked up {item.itemName}");
            return true;
        }

        public void DropItem()
        {
            // if(CurrentItem == null) return;
            // var tmp = CurrentItem;
            CurrentItem = null;
            // TODO: update UI
        }
    }
}
