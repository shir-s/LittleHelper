using Utils;

namespace Currency
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CoinPool : MonoSingleton<CoinPool>
    {
        public GameObject coinPrefab;
        public int poolSize = 25;

        private List<GameObject> availableCoins = new List<GameObject>();

        private void Awake()
        {
            CreatePool();
        }
        
        private void OnEnable()
        {
            GameEvents.RestartLevel += ResetPool;
        }
        private void OnDisable()
        {
            GameEvents.RestartLevel -= ResetPool;
        }

        private void CreatePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject coin = Instantiate(coinPrefab, transform);
                coin.SetActive(false);
                availableCoins.Add(coin);
            }
        }

        public GameObject GetCoin()
        {
            if (availableCoins.Count > 0)
            {
                GameObject coin = availableCoins[0];
                availableCoins.RemoveAt(0);
                coin.SetActive(true);
                return coin;
            }
            else
            {
                Debug.LogWarning("No coins available in the pool!");
                return null;
            }
        }

        public void ReturnCoin(GameObject coin)
        {
            coin.SetActive(false);
            availableCoins.Add(coin);
        }

        public void ResetPool()
        {
            foreach (Transform coin in transform)
            {
                coin.gameObject.SetActive(false);
                if (!availableCoins.Contains(coin.gameObject))
                    availableCoins.Add(coin.gameObject);
            }
        }
    }


}