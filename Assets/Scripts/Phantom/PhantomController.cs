using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PhantomColorManager))]
[RequireComponent(typeof(PhantomGunManager))]
[RequireComponent(typeof(PhantomHealthManager))]
[RequireComponent(typeof(PhantomStateManager))]
public class PhantomController : MonoBehaviour {

    [Header("References")]
    private PhantomHealthManager healthManager;
    private PhantomGunManager gunManager;
    private PhantomColorManager colorManager;
    private PhantomStateManager stateManager;
    private Rigidbody2D rb;

    [Header("Label")]
    [SerializeField] private string enemyName;
    [SerializeField] private TMP_Text nameText;

    [Header("Spawn")]
    private PhantomSpawn phantomSpawn;

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

    public void Initialize(PhantomSpawn phantomSpawn, Gun gun, bool isFlipped, Transform[] patrolPoints) {

        colorManager = GetComponent<PhantomColorManager>();
        gunManager = GetComponent<PhantomGunManager>();
        healthManager = GetComponent<PhantomHealthManager>();
        stateManager = GetComponent<PhantomStateManager>();
        rb = GetComponent<Rigidbody2D>();

        nameText.text = enemyName;

        this.phantomSpawn = phantomSpawn;

        gunManager.SetGun(gun);

        isFacingRight = !isFlipped;

        stateManager.Initialize(patrolPoints, isFlipped);

    }

    private void Update() {

        // if phantom is standing on something, claim it
        Collider2D leftCollider = Physics2D.OverlapCircle(leftFoot.position, groundCheckRadius, environmentMask);
        Collider2D rightCollider = Physics2D.OverlapCircle(rightFoot.position, groundCheckRadius, environmentMask);

        if (leftCollider != null)
            leftCollider.GetComponent<Claimable>()?.Claim(EntityType.Phantom, colorManager.GetCurrentPhantomColor().GetClaimColor());
        if (rightCollider != null)
            rightCollider.GetComponent<Claimable>()?.Claim(EntityType.Phantom, colorManager.GetCurrentPhantomColor().GetClaimColor());

    }

    public void CheckFlip() {

        if ((isFacingRight && rb.velocity.x < 0f) // phantom is going left while facing right
            || (!isFacingRight && rb.velocity.x > 0f)) // phantom is going right while facing left
            Flip();

    }

    public void Flip() {

        transform.Rotate(0f, 180f, 0f);
        healthManager.FlipCanvas();
        isFacingRight = !isFacingRight; // breaks when there are errors

    }

    public PhantomSpawn GetEnemySpawn() { return phantomSpawn; }

    public bool IsFacingRight() { return isFacingRight; }

}