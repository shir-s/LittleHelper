using System.Collections.Generic;
using UnityEngine;

namespace Rooms
{
    public class RoomFactory : MonoBehaviour
    {
        [SerializeField] private WorldConfig config;
        [SerializeField] private Transform roomsParent;

        
        public GameObject InstantiateRoom(RoomType type, Vector2 dir)
        {
            GameObject roomPrefab;
            switch (type)
            {
                case RoomType.Pantry:
                    roomPrefab = RandomFrom(config.pantryLayouts);
                    break;
                case RoomType.Freezer:
                    roomPrefab = RandomFrom(config.freezerLayouts);
                    break;
                case RoomType.Garden:
                    roomPrefab = RandomFrom(config.gardenLayouts);
                    break;
                default:
                    return null;
            }
            return Instantiate(roomPrefab, dir * config.roomSpacing, Quaternion.identity, roomsParent);
        }
        
        private GameObject RandomFrom(List<GameObject> list) => list[Random.Range(0, list.Count)];

    }
}