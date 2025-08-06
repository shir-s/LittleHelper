using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace UI
{
    public class HeartsHealthUI : MonoBehaviour
    {
        public GameObject heartPrefab;
        public Transform heartContainer;

        private List<GameObject> hearts = new List<GameObject>();
        private int maxHealth = 0;

        private void OnEnable()
        {
            GameEvents.PlayerHealthChanged += OnPlayerHealthChanged;
        }

        private void OnDisable()
        {
            GameEvents.PlayerHealthChanged -= OnPlayerHealthChanged;
        }

        private void OnPlayerHealthChanged(int current, int max)
        {
            if (max != maxHealth)
            {
                maxHealth = max;
                CreateHearts(maxHealth);
            }

            UpdateHearts(current);
        }

        private void CreateHearts(int count)
        {
            foreach (var heart in hearts)
            {
                Destroy(heart);
            }
            hearts.Clear();

            for (int i = 0; i < count; i++)
            {
                GameObject newHeart = Instantiate(heartPrefab, heartContainer);
                hearts.Add(newHeart);
            }
        }

        private void UpdateHearts(int current)
        {
            for (int i = 0; i < hearts.Count; i++)
            {
                hearts[i].SetActive(i < current);
            }
        }
    }
}