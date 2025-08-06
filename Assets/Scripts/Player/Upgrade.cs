using UnityEngine;

namespace Player
{
    public enum UpgradeType
    {
        ExtraHealth,
        SpeedBoost,
        FreezerTime,
        Hide
    }

    [CreateAssetMenu(menuName = "Upgrades/Upgrade")]
    public class Upgrade : ScriptableObject
    {
        public string upgradeName;
        public string description;
        public int cost;
        public UpgradeType type;
    }
}