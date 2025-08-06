using Enemies;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //[SerializeField] private GameObject enemyPrefab;     
    [SerializeField] private Transform[] spawnPoints;    
    //[SerializeField] private int baseAmount = 1;
    
    //private EnemyPool enemyPool; 
    private Rooms.Room parentRoom;

    private void Awake()
    {
        parentRoom = GetComponentInParent<Rooms.Room>();
        
    }

    public void SetRoom(Rooms.Room room)
    {
        parentRoom = room;
    }

    public void SpawnEnemies(EnemyPool enemyPool,int round, int baseAmount = 1)
    {
        int totalToSpawn = baseAmount * round;

        for (int i = 0; i < totalToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            //GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            GameObject enemy = enemyPool.GetEnemy(spawnPoint.position);

            if (parentRoom != null)
                parentRoom.RegisterEnemy(enemy);
        }
    }
}