using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static SpawnPoint Active { get; private set; }

    [Header("Spawn Settings")]
    public bool useAsDefault = true;
    public Vector3 spawnOffset;

    void Awake()
    {
        if (Active == null || useAsDefault)
            Active = this;
    }

    public Vector3 Position => transform.position + spawnOffset;
    public Quaternion Rotation => transform.rotation;
}
