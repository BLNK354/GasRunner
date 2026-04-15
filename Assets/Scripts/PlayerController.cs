using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead) return;

        // Auto-move forward
        rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

        // Jump logic
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver("Hit an obstacle!");
            Die();
        }
        else if (collision.CompareTag("Fuel"))
        {
            FuelSystem.Instance.Refill(20f);
            Destroy(collision.gameObject);
        }
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
    }
}