// PlayerController.cs - Gas Runner
// Handles player input, Rigidbody2D physics, jumping, and obstacle responses.
// Attach to the Player GameObject. Requires: Rigidbody2D, Collider2D, Animator.

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed = 5f;
    public float jumpForce = 10f;
    public float speedIncrease = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Status Effects")]
    public float slowMultiplier = 0.5f;
    public float slowDuration = 2.0f;
    public float disruptDuration = 1.5f;
    public float disruptMultiplier = 0.6f;
    public float hitPenaltyMultiplier = 0.75f;
    public float hitPenaltyDuration = 1.25f;

    Rigidbody2D rb;
    Animator anim;
    bool isGrounded;
    float currentSpeed;
    float slowTimer;
    float disruptTimer;
    float hitPenaltyTimer;
    bool isAlive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        if (!isAlive) return;
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing) return;

        HandleJumpInput();
        RampUpSpeed();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        CheckGrounded();
        TickStatusEffects();
        MoveForward();
    }

    void MoveForward()
    {
        float effectiveSpeed = currentSpeed * GameSettings.PlayerSpeedMultiplier;

        if (disruptTimer > 0)
            effectiveSpeed *= disruptMultiplier;

        rb.linearVelocity = new Vector2(effectiveSpeed, rb.linearVelocity.y);
        GameManager.Instance?.AddScore(1);
    }

    void RampUpSpeed()
    {
        if (slowTimer <= 0 && hitPenaltyTimer <= 0)
            baseSpeed += speedIncrease * Time.deltaTime;
    }

    void HandleJumpInput()
    {
        if (isGrounded && (Input.GetKeyDown(GameSettings.JumpKey) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * GameSettings.JumpForceMultiplier);
            anim?.SetTrigger("Jump");
            Debug.Log("[Player] Jumped!");
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        anim?.SetBool("IsGrounded", isGrounded);
    }

    void TickStatusEffects()
    {
        if (slowTimer > 0)
        {
            slowTimer -= Time.fixedDeltaTime;
            currentSpeed = baseSpeed * slowMultiplier;
            if (slowTimer <= 0)
            {
                slowTimer = 0f;
                currentSpeed = baseSpeed;
                Debug.Log("[Player] Speed restored.");
            }
        }

        if (hitPenaltyTimer > 0)
        {
            hitPenaltyTimer -= Time.fixedDeltaTime;
            if (hitPenaltyTimer < 0)
                hitPenaltyTimer = 0f;
        }

        if (disruptTimer > 0)
        {
            disruptTimer -= Time.fixedDeltaTime;
            if (disruptTimer < 0)
                disruptTimer = 0f;
        }

        if (slowTimer <= 0)
        {
            currentSpeed = baseSpeed;

            if (hitPenaltyTimer > 0)
                currentSpeed *= hitPenaltyMultiplier;
        }
    }

    public void OnHitPothole()
    {
        if (slowTimer > 0) return;
        slowTimer = slowDuration;
        ApplyHitPenalty();
        Debug.Log("[Player] Hit POTHOLE - slowed!");
        anim?.SetTrigger("Stumble");
        SoundManager.Instance?.PlayObstacleHit();
    }

    public void OnHitPuddle()
    {
        disruptTimer = disruptDuration;
        ApplyHitPenalty();
        Debug.Log("[Player] Hit PUDDLE - disrupted!");
        anim?.SetTrigger("Stumble");
        SoundManager.Instance?.PlayObstacleHit();
    }

    public void OnHitBarrier()
    {
        isAlive = false;
        rb.linearVelocity = Vector2.zero;
        anim?.SetTrigger("Die");
        Debug.Log("[Player] Hit BARRIER - game over!");
        SoundManager.Instance?.PlayObstacleHit();
        GameManager.Instance?.SetState(GameManager.GameState.GameOver);
    }

    void ApplyHitPenalty()
    {
        hitPenaltyTimer = Mathf.Max(hitPenaltyTimer, hitPenaltyDuration);
    }

    void UpdateAnimator()
    {
        anim?.SetFloat("Speed", currentSpeed);
        anim?.SetBool("IsGrounded", isGrounded);
    }

    public bool IsAlive => isAlive;
    public bool IsGrounded => isGrounded;
    public float CurrentSpeed => currentSpeed;
}
