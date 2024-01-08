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
    private Rigidbody2D rb;

    [Header("Movement")]
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

    private void Awake() {

        colorManager = GetComponent<EnemyColorManager>();
        enemyHealth = GetComponent<EnemyHealthManager>();
        rb = GetComponent<Rigidbody2D>();

        isFacingRight = true;

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

        if ((isFacingRight && rb.velocity.x < 0f) || (!isFacingRight && rb.velocity.x > 0f))
            Flip();

    }

    public void Flip() {

        transform.Rotate(0f, 180f, 0f);
        enemyHealth.FlipCanvas();
        isFacingRight = !isFacingRight; // breaks when there are errors

    }

    public bool IsFacingRight() { return isFacingRight; }

}