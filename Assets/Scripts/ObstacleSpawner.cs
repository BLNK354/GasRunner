using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstacles; 
    public GameObject fuelPrefab;
    public Transform player;
    
    private float spawnX = 10f;
    private float distanceBetween = 15f;

    void Update()
    {
        // Spawns obstacles ahead of the player position
        if (spawnX < player.position.x + 30f)
        {
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        GameObject toSpawn = (Random.value > 0.8f) ? fuelPrefab : obstacles[Random.Range(0, obstacles.Length)];
        Instantiate(toSpawn, new Vector3(spawnX, -3.5f, 0), Quaternion.identity);
        spawnX += distanceBetween;
    }
}