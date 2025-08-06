using System;
using Managers;
using Rooms;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Currency
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private LayerMask forbiddenLayer; 
        [SerializeField] private int totalCoinsToSpawn = 20;
        [SerializeField] private float spawnRadius = 0.3f;

        private List<Room> _rooms;
        
        private void Start()
        {
            // _rooms = GameManager.Instance.Rooms;
            // InitialSpawn();
        }

        private void OnEnable()
        {
            //GameEvents.RestartLevel += HandleRestart;
            GameEvents.OnCoinCollected += OnCoinCollected;
        }
        
        private void OnDisable()
        {
            //GameEvents.RestartLevel -= HandleRestart;
            GameEvents.OnCoinCollected -= OnCoinCollected;
        }

        /*private void HandleRestart()
        {
            print("not implemented");
        }*/
        
        public void SetRooms(List<Room> rooms)
        {
            this._rooms = rooms;
        }

        public void InitialSpawn()
        {
            this._rooms = GameManager.Instance.Rooms;

            for (int i = 0; i < totalCoinsToSpawn; i++)
            {
                SpawnSingleCoin();
            }
        }

        private void SpawnSingleCoin()
        {
            
            if (_rooms.Count == 0) return;
            Room randomRoom = _rooms[Random.Range(0, _rooms.Count)];
            Vector2 spawnPosition = FindValidPositionInRoom(randomRoom);

            if (spawnPosition != Vector2.zero)
            {
                GameObject coin = CoinPool.Instance.GetCoin();
                if (coin != null)
                {
                    coin.transform.position = spawnPosition;
                    coin.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning("No valid position found for coin.");
            }
        }

        private Vector2 FindValidPositionInRoom(Room room)
        {
            Collider2D roomCollider = room.GetComponent<Collider2D>();
            if (roomCollider == null)
            {
                Debug.LogWarning("Room has no collider!");
                return Vector2.zero;
            }

            Bounds bounds = roomCollider.bounds;

            for (int i = 0; i < 10; i++)
            {
                Vector2 randomPos = new Vector2(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y)
                );

                if (!Physics2D.OverlapCircle(randomPos, spawnRadius, forbiddenLayer))
                {
                    return randomPos;
                }
            }

            return Vector2.zero; 
        }

        public void OnCoinCollected()
        {
            SpawnSingleCoin();
        }
    }
}
