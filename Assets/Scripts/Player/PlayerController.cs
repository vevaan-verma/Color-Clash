using DG.Tweening;
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
    private CameraController cameraController;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Control")]
    private bool hasControl;

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

    [Header("Rotation")]
    private bool isRotated;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey;

    /*
    IMPORTANT:
        - PLAYER MUST START FACING RIGHT
    */

    private void Start() {

        cameraController = FindObjectOfType<CameraController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        isFacingRight = true;

        hasControl = true;

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

        if (Input.GetKeyUp(jumpKey) && (isRotated ? -1f : 1f) * rb.velocity.y > 0f) // if jump is let go in the air, player falls quicker, adjust input based on rotation
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

    }

    private void FixedUpdate() {

        rb.velocity = new Vector2((isRotated ? -1f : 1f) * horizontalInput * moveSpeed, rb.velocity.y); // adjust input based on rotation

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

        rb.velocity = transform.up * new Vector2(rb.velocity.x, jumpForce); // multiply by transform.up to make sure jump is always up relative to player

    }

    public void FlipPlayer(float rotationDuration) {

        if (isRotated)
            transform.rotation = Quaternion.identity;
        else
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 180f);

        cameraController.RotateCamera(rotationDuration, isRotated);
        isRotated = !isRotated;

    }

    public void ResetPlayer() {

        transform.rotation = Quaternion.identity;
        cameraController.ResetCamera();
        isRotated = false;
        isFacingRight = true; // player will always start facing right

    }

    public Transform GetLeftFoot() { return leftFoot; }

    public Transform GetRightFoot() { return rightFoot; }

    public LayerMask GetEnvironmentMask() { return environmentMask; }

    public bool HasControl() { return hasControl; }

    public void SetHasControl(bool hasControl) { this.hasControl = hasControl; }

}