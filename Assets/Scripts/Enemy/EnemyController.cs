using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(EnemyColorManager))]
[RequireComponent(typeof(EnemyGunManager))]
[RequireComponent(typeof(EnemyHealthManager))]
[RequireComponent(typeof(EnemyStateManager))]
public class EnemyController : MonoBehaviour {

    [Header("References")]
    private EnemyHealthManager enemyHealth;
    private EnemyColorManager colorManager;
    private EnemyStateManager stateManager;
    private Rigidbody2D rb;

    [Header("Spawn")]
    private EnemySpawn enemySpawn;

    [Header("Movement")]
    private bool flipped;
    private bool isFacingRight;

    [Header("Ground Check")]
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private float groundCheckRadius;

    /*
    IMPORTANT:
        - ENEMY MUST START FACING RIGHT
        - VISION OBJECT CANNOT BE ON ENEMY LAYER
    */

    public void Initialize(EnemySpawn enemySpawn, bool flipped, Transform[] patrolPoints) {

        colorManager = GetComponent<EnemyColorManager>();
        enemyHealth = GetComponent<EnemyHealthManager>();
        stateManager = GetComponent<EnemyStateManager>();
        rb = GetComponent<Rigidbody2D>();

        this.enemySpawn = enemySpawn;

        stateManager.SetPatrolPoints(patrolPoints);

        this.flipped = flipped;
        isFacingRight = !flipped;

    }

    private void Update() {

        // if enemy is standing on something, claim it
        Collider2D leftCollider = Physics2D.OverlapCircle(leftFoot.position, groundCheckRadius, environmentMask);
        Collider2D rightCollider = Physics2D.OverlapCircle(rightFoot.position, groundCheckRadius, environmentMask);

        if (leftCollider != null)
            leftCollider.GetComponent<Claimable>()?.Claim(EntityType.Enemy, colorManager.GetCurrentEnemyColor().GetClaimColor());
        if (rightCollider != null)
            rightCollider.GetComponent<Claimable>()?.Claim(EntityType.Enemy, colorManager.GetCurrentEnemyColor().GetClaimColor());

    }

    public void CheckFlip() {

        if ((flipped && ((isFacingRight && rb.velocity.x > 0f) // flipped player is going LEFT (because they're flipped) while facing right
            || (!isFacingRight && rb.velocity.x < 0f))) // flipped player is going RIGHT (because they're flipped) while facing left
            || (!flipped && ((isFacingRight && rb.velocity.x < 0f) // unflipped player is going left while facing right
            || (!isFacingRight && rb.velocity.x > 0f)))) // unflipped player is going right while facing left
            Flip();

    }

    public void Flip() {

        transform.Rotate(0f, 180f, 0f);
        enemyHealth.FlipCanvas();
        isFacingRight = !isFacingRight; // breaks when there are errors

    }

    public EnemySpawn GetEnemySpawn() { return enemySpawn; }

    public bool IsFacingRight() { return isFacingRight; }

}