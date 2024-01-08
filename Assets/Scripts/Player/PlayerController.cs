using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerClaimManager))]
[RequireComponent(typeof(PlayerColorManager))]
[RequireComponent(typeof(PlayerEffectManager))]
[RequireComponent(typeof(PlayerGunManager))]
[RequireComponent(typeof(PlayerHealthManager))]
public class PlayerController : MonoBehaviour {

    [Header("References")]
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private float horizontalInput;
    private bool isFacingRight;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;

    [Header("Ground Check")]
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private float groundCheckRadius;
    private bool isGrounded;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey;

    /*
    IMPORTANT:
        - PLAYER MUST START FACING RIGHT
    */

    private void Start() {

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        isFacingRight = true;

    }

    private void Update() {

        // ground check
        isGrounded = Physics2D.OverlapCircle(leftFoot.position, groundCheckRadius, environmentMask) != null || Physics2D.OverlapCircle(rightFoot.position, groundCheckRadius, environmentMask) != null; // check both feet for ground check

        // movement
        horizontalInput = Input.GetAxisRaw("Horizontal");
        CheckFlip();

        // jumping
        if (Input.GetKey(jumpKey) && isGrounded)
            Jump();

        if (Input.GetKeyUp(jumpKey) && rb.velocity.y > 0f) // if jump is let go in the air, player falls quicker
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

    }

    private void FixedUpdate() {

        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        if (horizontalInput != 0f && isGrounded) // player is moving on ground
            animator.SetBool("isRunning", true);
        else
            animator.SetBool("isRunning", false);

    }

    private void CheckFlip() {

        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f) {

            transform.Rotate(0f, 180f, 0f);
            isFacingRight = !isFacingRight; // breaks when there are errors

        }
    }

    private void Jump() {

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    }

    public Transform GetLeftFoot() { return leftFoot; }

    public Transform GetRightFoot() { return rightFoot; }

    public LayerMask GetEnvironmentMask() { return environmentMask; }

}