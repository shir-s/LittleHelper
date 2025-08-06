using Enemies;
using Utils;

namespace Managers
{
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyPoolManager : MonoSingleton<EnemyPoolManager>
    {
        //public static EnemyPoolManager Instance { get; private set; }

        private Dictionary<EnemyType, EnemyPool> pools = new Dictionary<EnemyType, EnemyPool>();

        private void Awake()
        {
            EnemyPool[] allPools = GetComponentsInChildren<EnemyPool>(true);
            foreach (var pool in allPools)
            {
                if (!pools.ContainsKey(pool.Type))
                {
                    pools.Add(pool.Type, pool);
                }
            }
        }
        
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
            foreach (var pool in pools.Values)
            {
                pool.ResetPool();
            }
        }

        public EnemyPool GetPool(EnemyType type)
        {
            if (type == EnemyType.None)
                return null;
            if (pools.TryGetValue(type, out var pool))
            {
                return pool;
            }
            Debug.LogError($"No pool found for enemy type {type}");
            return null;
        }
    }

}