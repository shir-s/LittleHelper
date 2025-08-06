using System;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using Managers;
using Scriptable_Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rooms
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private List<Interactable> containers;
        [SerializeField] private bool hasEnemies = false;
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private EnemyType enemyTypeNeededForThisRoom;
        public RoomType RoomType {get; set;}

        private int _fullContainers;
        public int TotalContainers  => containers.Count;
        public int FilledContainers => _fullContainers;
        private bool AllContainersFull => _fullContainers == containers.Count;
        private bool _playerInside;
        private int _currentRound = 1;
        private EnemyPool _enemyPool;
        private int _enemyBaseAmount;
        private FreezingRoomEnemy _freezingRoomEnemy = null;
        
        private List<IEnemy> _enemiesInRoom = new();

        private void Awake()
        {
            if (enemySpawner == null)
            {
                if (hasEnemies)
                {
                    enemySpawner = GetComponentInChildren<EnemySpawner>();
                    if (enemySpawner == null)
                    {
                        Debug.LogError("EnemySpawner is missing in this room with enemies!");
                    }
                }
            }

            if (hasEnemies && enemySpawner != null)
            {
                enemySpawner.SetRoom(this);
            }

            IEnemy[] existingEnemies = GetComponentsInChildren<IEnemy>(includeInactive: true);
            foreach (var enemy in existingEnemies)
            {
                _enemiesInRoom.Add(enemy);
            }
        }

        private void Start()
        {
            if (hasEnemies)
            {
                _enemyPool = EnemyPoolManager.Instance.GetPool(enemyTypeNeededForThisRoom);
            }

            switch (enemyTypeNeededForThisRoom)
            {
                case EnemyType.None:
                    hasEnemies = false;
                    _freezingRoomEnemy = GetComponent<FreezingRoomEnemy>();
                    break;
                case EnemyType.Rat:
                    _enemyBaseAmount = 2;
                    break;
                case EnemyType.Mole:
                    _enemyBaseAmount = 1;
                    break;
            }
            
        }


        public void RegisterEnemy(GameObject enemyObj)
        {
            IEnemy enemy = enemyObj.GetComponent<IEnemy>();
            if (enemy != null && !_enemiesInRoom.Contains(enemy))
            {
                _enemiesInRoom.Add(enemy);
            }
        }

        public void OnRoundStarted(int currentRound)
        {
            this._currentRound = currentRound;
            if(_freezingRoomEnemy != null)
            {
                print("FreezingRoomEnemy found");
                _freezingRoomEnemy.OnRoundStarted(currentRound);
            }
        }
        private void UpdateEnemyLevel(int round)
        {
            foreach (var enemy in _enemiesInRoom)
            {
                enemy.OnRoundStarted(round);
            }
        }

        public void AddItemToRandomContainer(ItemDefinition item)
        {
            if (AllContainersFull) return;
            GetRandomEmptyContainer()?.AddItem(item);
        }

        private Interactable GetRandomEmptyContainer()
        {
            // var emptyContainers = containers.Where(c => c.IsEmpty).ToList();
            var emptyContainers = containers
                .Where(c => c != null && c.IsEmpty)
                .ToList();
            if (emptyContainers.Count == 0) return null;
            int index = Random.Range(0, emptyContainers.Count);
            return emptyContainers[index];
        }
        
        public void OnPlayerExited()
        {
            foreach (var enemy in _enemiesInRoom)
            {
                if (enemy is MonoBehaviour enemyMono) 
                {
                    _enemyPool.ReturnEnemy(enemyMono.gameObject);
                }
            }
            _enemiesInRoom.Clear();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = true;
                if (enemySpawner != null && hasEnemies && _enemyPool != null)
                {
                    enemySpawner.SpawnEnemies(_enemyPool, _currentRound * 2, _enemyBaseAmount);
                }
                
                UpdateEnemyLevel(_currentRound);
            }

            switch (enemyTypeNeededForThisRoom)
            {
                case EnemyType.None:
                    //play freezer sound
                    SoundManager.Instance.PlayFreezerAmbience();
                    break;
                case EnemyType.Rat:
                    // play PlayRatsAmbience
                    SoundManager.Instance.PlayRatsAmbience();
                    break;
                case EnemyType.Mole:
                    // play PlayGardenAmbience
                    SoundManager.Instance.PlayGardenAmbience();
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = false;
                if (hasEnemies && _enemyPool != null)
                {
                    UpdateEnemyLevel(1);
                    OnPlayerExited();
                }
            }
            //SoundManager.Instance.StopAmbience();
            switch (enemyTypeNeededForThisRoom)
            {
                case EnemyType.None:
                    SoundManager.Instance.StopFreezerAmbience();
                    break;
                case EnemyType.Rat:
                    SoundManager.Instance.StopRatsAmbience();
                    break;
                case EnemyType.Mole:
                    SoundManager.Instance.StopGardenAmbience();
                    break;
            }

        }

    }
}