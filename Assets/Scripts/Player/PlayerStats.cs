using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerStats
    {
        [Header("Health Settings")]
        public int maxHealth = 100;

        [Header("Movement Settings")]
        public float moveSpeed = 5f;

        [Header("Hide Settings")]
        public float hideDuration = 3f;
        public float hideCooldown = 3f;
        public bool hasHideUpgrade = false;


        [Header("Freezer Upgrade Settings")]
        public float baseFreezeTime = 10f;
        public float extraFreezeTime = 0f;

        public float TotalFreezeTime => baseFreezeTime + extraFreezeTime;

    }
}