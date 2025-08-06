using UnityEngine;

public interface IEnemy
{
    
    void AttackPlayer(GameObject player);
    void OnRoundStarted(int level);
}
