using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerClaimManager))]
[RequireComponent(typeof(PlayerColorManager))]
[RequireComponent(typeof(PlayerEffectManager))]
[RequireComponent(typeof(PlayerGunManager))]
[RequireComponent(typeof(PlayerHealthManager))]
public class PlayerController : MonoBehaviour {

    [Header("References")]
    private CameraController cameraController;
    private UIController uiController;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Mechanics")]
    private Dictionary<MechanicType, bool> mechanicStatuses;

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
    [SerializeField] private KeyCode pauseKey;

    /*
    IMPORTANT:
        - PLAYER MUST START FACING RIGHT
    */

    public void Initialize(UIController uiController) { // to initialize ui controller

        this.uiController = uiController;

    }

    private void Awake() {

        // set up mechanic statuses early so scripts can change them earlier too
        mechanicStatuses = new Dictionary<MechanicType, bool>();
        Array mechanics = Enum.GetValues(typeof(MechanicType)); // get all mechanic type values

        // add all mechanic types to dictionary
        foreach (MechanicType mechanicType in mechanics)
            if (mechanicType != MechanicType.None) // ignore none mechanic type
                mechanicStatuses.Add(mechanicType, false); // set all mechanics to false by default

    }

    private void Start() {

        cameraController = FindObjectOfType<CameraController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        isFacingRight = true;

    }

    private void Update() {

        // ground check
        isGrounded = Physics2D.OverlapCircle(leftFoot.position, groundCheckRadius, environmentMask) != null || Physics2D.OverlapCircle(rightFoot.position, groundCheckRadius, environmentMask) != null; // check both feet for ground check (overlap circle allows for mantle mechanic)

        // movement
        if (IsMechanicEnabled(MechanicType.Movement)) {

            horizontalInput = Input.GetAxisRaw("Horizontal");
            CheckFlip();

        }

        // jumping
        if (IsMechanicEnabled(MechanicType.Jumping)) {

            if (Input.GetKey(jumpKey) && isGrounded)
                Jump();

            if (Input.GetKeyUp(jumpKey) && (isRotated ? -1f : 1f) * rb.velocity.y > 0f) // if jump is let go in the air, player falls quicker, adjust input based on rotation
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

        }

        // pausing
        if (Input.GetKeyDown(pauseKey))
            uiController.TogglePause();

    }

    private void FixedUpdate() {

        if (IsMechanicEnabled(MechanicType.Movement)) { // don't return if false to allow for more code to be added to this method later

            rb.velocity = new Vector2((isRotated ? -1f : 1f) * horizontalInput * moveSpeed, rb.velocity.y); // adjust input based on rotation

            if (horizontalInput != 0f && isGrounded) // player is moving on ground
                animator.SetBool("isRunning", true);
            else
                animator.SetBool("isRunning", false);

        }
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

    public bool IsMechanicEnabled(MechanicType mechanicType) {

        if (mechanicType == MechanicType.None)
            return false; // don't allow none mechanic type to be checked

        return mechanicStatuses[mechanicType];

    }

    public void EnableMechanic(MechanicType mechanicType) {

        mechanicStatuses[mechanicType] = true;

        // enable mechanic related UI
        switch (mechanicType) {

            case MechanicType.Guns:
                uiController.EnableGunCycleHUD();
                uiController.EnableHealthBarHUD();
                break;

            case MechanicType.Claiming:
                uiController.EnableClaimablesInfoHUD();
                break;

        }
    }

    public void EnableAllMechanics() {

        // set all mechanics in dictionary to true
        foreach (MechanicType mechanicType in mechanicStatuses.Keys.ToList()) // use ToList() to avoid InvalidOperationException
            mechanicStatuses[mechanicType] = true;

        // enable all mechanics related UI
        uiController.EnableClaimablesInfoHUD();
        uiController.EnableGunCycleHUD();
        uiController.EnableHealthBarHUD();

    }

    public void DisableMechanic(MechanicType mechanicType) {

        mechanicStatuses[mechanicType] = false;

        // disable mechanic related UI
        switch (mechanicType) {

            case MechanicType.Guns:
                uiController.DisableGunCycleHUD();
                uiController.DisableHealthBarHUD();
                break;

            case MechanicType.Claiming:
                uiController.DisableClaimablesInfoHUD();
                break;

            case MechanicType.Movement:
                animator.SetBool("isRunning", false); // stop running animation
                break;

        }
    }

    public void DisableAllMechanics() {

        // set all mechanics in dictionary to false
        foreach (MechanicType mechanicType in mechanicStatuses.Keys.ToList()) // use ToList() to avoid InvalidOperationException
            mechanicStatuses[mechanicType] = false;

        animator.SetBool("isRunning", false); // stop running animation

        // disable all mechanics related UI
        uiController.DisableClaimablesInfoHUD();
        uiController.DisableGunCycleHUD();
        uiController.DisableHealthBarHUD();

    }
}