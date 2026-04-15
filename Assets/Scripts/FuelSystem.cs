using UnityEngine;

public class FuelSystem : MonoBehaviour
{
    public static FuelSystem Instance;
    public float currentFuel = 100f;
    public float fuelBurnRate = 5f;
    public float maxFuel = 100f;

    void Awake() => Instance = this;

    void Update()
    {
        if (!GameManager.Instance.isGameActive) return;

        currentFuel -= fuelBurnRate * Time.deltaTime;
        
        if (currentFuel <= 0)
        {
            currentFuel = 0;
            GameManager.Instance.GameOver("Out of Gas!");
        }
    }

    public void Refill(float amount)
    {
        currentFuel = Mathf.Clamp(currentFuel + amount, 0, maxFuel);
    }
}