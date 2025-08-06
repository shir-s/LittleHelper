using System.Collections.Generic;
using System.Linq;
using Managers;
using Rooms;
using Scriptable_Objects;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Item
{
    public static class ItemPlacer
    {
        private static float FILL_EMPTY_CHANCE = 0.7f; 
        public static void PopulateContainers(List<ItemDefinition> recipeItems,List<ItemDefinition> trashItems, List<Room> rooms)
        {
            // foreach (var item in items)
            // {
            //     foreach (var room in rooms.Where(room => item.roomType == room.RoomType))
            //     {
            //         room.AddItemToRandomContainer(item);
            //     }
            // }

            foreach (var item in recipeItems)
            {
                foreach (var room in rooms.Where(room => item.roomType == room.RoomType))
                {
                    room.AddItemToRandomContainer(item);
                }
            }

            foreach (var room in rooms)
            {
                var emptyContainersAmount = room.TotalContainers - room.FilledContainers;
                for (var i = 0; i < emptyContainersAmount; i++)
                {
                    if (Random.value < FILL_EMPTY_CHANCE)
                    {
                        var trashItemsForRoom = trashItems.Where(trashItem => trashItem.roomType == room.RoomType).ToList();
                        room.AddItemToRandomContainer(trashItemsForRoom[Random.Range(0, trashItemsForRoom.Count)]);
                    }
                }

            }
        }
    }
}