// ObstacleSpawner.cs — Gas Runner
// Spawns obstacles ahead of the player and handles trigger collisions.
// Attach to an empty GameObject named "ObstacleSpawner" in the level scene.

using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    // ------------------------------------------------------------------ //
    //  INSPECTOR SETTINGS                                                  //
    // ------------------------------------------------------------------ //
    [Header("Obstacle Prefabs")]
    public GameObject potholePrefab;    // Flat ground hazard
    public GameObject puddlePrefab;     // Slippery surface
    public GameObject barrierPrefab;    // Wall — must be jumped over

    [Header("Spawn Settings")]
    public float spawnInterval   = 3f;   // Seconds between spawns
    public float spawnAheadX     = 20f;  // How far ahead to spawn
    public float despawnBehindX  = -5f;  // Remove obstacles behind this X offset

    [Header("Spawn Weights (must add up)")]
    [Range(0f, 1f)] public float potholeWeight = 0.40f;
    [Range(0f, 1f)] public float puddleWeight  = 0.40f;
    [Range(0f, 1f)] public float barrierWeight = 0.20f;

    [Header("References")]
    public Transform playerTransform;

    // ------------------------------------------------------------------ //
    //  PRIVATE STATE                                                       //
    // ------------------------------------------------------------------ //
    float spawnTimer;

    // ------------------------------------------------------------------ //
    //  UNITY LIFECYCLE                                                     //
    // ------------------------------------------------------------------ //
    void Start()
    {
        spawnTimer = spawnInterval;

        // Auto-find player if not assigned
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnInterval;
            SpawnObstacle();
        }

        DespawnOldObstacles();
    }

    // ------------------------------------------------------------------ //
    //  SPAWNING                                                            //
    // ------------------------------------------------------------------ //
    void SpawnObstacle()
    {
        GameObject prefab = PickObstaclePrefab();
        if (prefab == null) return;

        float spawnX = playerTransform.position.x + spawnAheadX;
        float spawnY = 0f;   // Ground level — adjust per obstacle type in prefab

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
        Instantiate(prefab, spawnPos, Quaternion.identity);

        Debug.Log($"[Spawner] Spawned {prefab.name} at x={spawnX:F1}");
    }

    GameObject PickObstaclePrefab()
    {
        // Weighted random selection — matches Python obstacles.py logic
        float roll = Random.value;   // 0.0 to 1.0

        if (roll < potholeWeight)
            return potholePrefab;
        else if (roll < potholeWeight + puddleWeight)
            return puddlePrefab;
        else
            return barrierPrefab;
    }

    // ------------------------------------------------------------------ //
    //  DESPAWNING                                                          //
    // ------------------------------------------------------------------ //
    void DespawnOldObstacles()
    {
        // Find all obstacle GameObjects and remove ones behind the player
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var obs in obstacles)
        {
            if (obs.transform.position.x < playerTransform.position.x + despawnBehindX)
            {
                Destroy(obs);
            }
        }
    }
}

// ================================================================== //
//  OBSTACLE BEHAVIOUR SCRIPTS                                         //
//  Attach one of these to each obstacle prefab.                      //
// ================================================================== //

/// <summary>Pothole — slows player down.</summary>
public class Pothole : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.OnHitPothole();
            // Don't destroy — player runs over it
        }
    }
}

/// <summary>Puddle — disrupts player controls.</summary>
public class Puddle : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.OnHitPuddle();
        }
    }
}

/// <summary>Barrier — solid wall. Collision (not trigger).</summary>
public class Barrier : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {
        // Barrier uses Collision (solid), not Trigger
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>()?.OnHitBarrier();
        }
    }
}