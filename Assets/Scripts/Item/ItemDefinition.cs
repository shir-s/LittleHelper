using Rooms;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable_Objects
{
    [CreateAssetMenu(
        fileName = "ItemDefinition",
        menuName = "Game/ Item Definition",
        order = 1)]
    public class ItemDefinition : ScriptableObject
    {
        public string itemName;
        [Tooltip("icon shown in UI")]
        public Sprite icon;
        [Tooltip("prefab spawned in the world")]
        public GameObject prefab;
        [Tooltip("Room where the item will be spawned")]
        public RoomType roomType;

        [Tooltip("Position where the item will be placed on delivery (leave empty if trash item")]
        public Vector3 deliveryPosition;
        public Vector3 scale;
        public Quaternion rotation;
    }
}
