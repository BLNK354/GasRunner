using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public SpawnPoint spawnPoint;

    [Header("Options")]
    public bool spawnOnStart = true;

    void Start()
    {
        if (spawnOnStart)
            SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (spawnPoint == null)
            spawnPoint = SpawnPoint.Active;

        if (playerTransform == null || spawnPoint == null) return;

        playerTransform.SetPositionAndRotation(spawnPoint.Position, spawnPoint.Rotation);

        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}
