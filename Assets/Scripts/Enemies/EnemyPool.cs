namespace Enemies
{
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private EnemyType enemyType;
        public EnemyType Type => enemyType;
        private Queue<GameObject> availableEnemies = new Queue<GameObject>();
        private readonly List<GameObject> activeEnemies = new List<GameObject>();

        
        public GameObject GetEnemy(Vector2 position)
        {
            GameObject enemy;
            if (availableEnemies.Count > 0)
            {
                enemy = availableEnemies.Dequeue();
                //enemy.SetActive(true);
            }
            else
            {
                enemy = Instantiate(enemyPrefab);
            }

            enemy.transform.position = position;
            enemy.SetActive(true);
            activeEnemies.Add(enemy);
            return enemy;
        }

        public void ReturnEnemy(GameObject enemy)
        {
            enemy.SetActive(false);
            if (!availableEnemies.Contains(enemy))
                availableEnemies.Enqueue(enemy);
            activeEnemies.Remove(enemy);
        }
        
        public void ResetPool()
        {
            foreach (var enemy in activeEnemies.ToArray())
            {
                ReturnEnemy(enemy);
            }
            activeEnemies.Clear();
        }

    }

}