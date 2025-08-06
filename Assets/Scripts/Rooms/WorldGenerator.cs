using System.Collections.Generic;
using Door;
using Managers;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rooms
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject kitchenPrefab;
        [SerializeField] private RoomFactory roomFactory;
        private GameObject _roomLeft, _roomRight, _roomThird;
        
   
        public List<Room> GenerateWorld()
        {
            var types = new List<RoomType> { RoomType.Pantry, RoomType.Freezer, RoomType.Garden };
            RoomType rightRoomType = PopRandom(types);
            RoomType leftRoomType = PopRandom(types);
            RoomType thirdRoomType = types[0];

            _roomRight = SetUpRoom(rightRoomType,Vector2.right);
            
            _roomLeft = SetUpRoom(leftRoomType,Vector2.left);
            
            bool attachedRight = Random.value < 0.5f;
            Vector2 dir = attachedRight ? Vector2.right : Vector2.left;
            _roomThird = SetUpRoom(thirdRoomType, dir * 2);
            
            Connect(kitchenPrefab, DoorSide.Right, _roomRight, DoorSide.Left);
            
            Connect(kitchenPrefab, DoorSide.Left, _roomLeft, DoorSide.Right);
            
            if (attachedRight)
            {
                Connect (_roomRight, DoorSide.Right, _roomThird, DoorSide.Left);
                DisableDoor(_roomLeft, DoorSide.Left);
                DisableDoor(_roomThird, DoorSide.Right);
            }
            else
            {
                Connect(_roomLeft,DoorSide.Left, _roomThird, DoorSide.Right);
                DisableDoor(_roomRight, DoorSide.Right);
                DisableDoor(_roomThird, DoorSide.Left);
            }
            
            return new List<Room> { _roomRight.GetComponent<Room>(), _roomLeft.GetComponent<Room>(), _roomThird.GetComponent<Room>()};
        }


        private GameObject SetUpRoom(RoomType roomType, Vector2 dir)
        {
            var room = roomFactory.InstantiateRoom(roomType, dir);
            SetUpRoomCamera(room);
            room.GetComponent<Room>().RoomType = roomType;
            return room;
        } 
        
        private void Connect(GameObject fromRoom, DoorSide fromSide, GameObject toRoom, DoorSide toSide)
        {
            var doorFrom = fromRoom.transform.Find($"Doors/{fromSide} Door");
            var doorTo = toRoom.transform.Find($"Doors/{toSide} Door");
            
            if (doorFrom == null || doorTo == null)
            {
                Debug.LogError($"Missing door: {fromSide} in {fromRoom.name} or {toSide} in {toRoom.name}");
            }
            
            var doorFromTrigger = doorFrom.GetComponent<DoorTrigger>();
            doorFromTrigger.targetPosition = doorTo.Find("EntryPoint");
            
            var doorToTrigger = doorTo.GetComponent<DoorTrigger>();
            doorToTrigger.targetPosition = doorFrom.Find("EntryPoint");
            
            var doorFromTransition = doorFrom.GetComponent<RoomTransition>();
            doorFromTransition.previousCamera = fromRoom.GetComponentInChildren<CinemachineCamera>();
            doorFromTransition.newCamera = toRoom.GetComponentInChildren<CinemachineCamera>();
            
            var doorToTransition = doorTo.GetComponent<RoomTransition>();
            doorToTransition.previousCamera = toRoom.GetComponentInChildren<CinemachineCamera>();
            doorToTransition.newCamera = fromRoom.GetComponentInChildren<CinemachineCamera>();
        }

        private void SetUpRoomCamera(GameObject roomGameObject)
        {
            var roomCamera = roomGameObject.GetComponentInChildren<CinemachineCamera>();
            if (roomCamera == null)
            {
                Debug.Log("camera not found");
            }
            var player = GameManager.Instance.PlayerObject;
            roomCamera.LookAt = player.transform;
            roomCamera.Follow = player.transform;
        }
        
        private void DisableDoor(GameObject room, DoorSide side)
        {
            var door = room.transform.Find($"Doors/{side} Door");
            if(door != null) door.gameObject.SetActive(false);
        }
        
        private static RoomType PopRandom(List<RoomType> types)
        {
            var i = Random.Range(0, types.Count);
            var roomType = types[i];
            types.RemoveAt(i);
            return roomType;
        }
    }
}


