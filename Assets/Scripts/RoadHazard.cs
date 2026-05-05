using UnityEngine;

public class RoadHazard : MonoBehaviour
{
    public enum HazardType
    {
        Pothole,
        Puddle,
        Barrier
    }

    [Header("Hazard")]
    public HazardType hazardType = HazardType.Pothole;
    public bool destroyAfterHit;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ApplyHazard(other.GetComponent<PlayerController>());
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        ApplyHazard(other.gameObject.GetComponent<PlayerController>());
    }

    void ApplyHazard(PlayerController player)
    {
        if (player == null) return;

        switch (hazardType)
        {
            case HazardType.Pothole:
                player.OnHitPothole();
                break;
            case HazardType.Puddle:
                player.OnHitPuddle();
                break;
            case HazardType.Barrier:
                player.OnHitBarrier();
                break;
        }

        if (destroyAfterHit)
            Destroy(gameObject);
    }
}
