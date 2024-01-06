using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    private new Collider2D collider;
    private PlayerHealth playerHealth;
    private UIController uiController;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private float horizontalInput;
    private bool isFacingRight;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;

    [Header("Guns")]
    [SerializeField] private List<Gun> starterGuns; // DON'T USE GUNS FROM THIS, THEY AREN'T INSTANTIATED
    [SerializeField] private Transform gunSlot;
    [SerializeField] private LayerMask shootableMask; // just to avoid player and bullet collisions
    private List<Gun> guns; // contains the actual instantiated guns
    private int currGunIndex;

    [Header("Ground Check")]
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private float groundCheckRadius;
    private bool isGrounded;

    [Header("Color Cycling")]
    [SerializeField] private PlayerSprite[] playerSprites;
    [SerializeField] private float colorCycleCooldown;
    private int currSpriteIndex;
    private SpriteRenderer spriteRenderer;
    private bool canColorCycle;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode colorSwitchKey;
    [SerializeField] private KeyCode reloadKey;

    [Header("Quitting")]
    private bool quitting;

    private void Start() {

        collider = GetComponent<Collider2D>();
        playerHealth = GetComponent<PlayerHealth>();
        uiController = FindObjectOfType<UIController>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currSpriteIndex = 0;
        spriteRenderer.sprite = playerSprites[currSpriteIndex].GetSprite();

        guns = new List<Gun>();

        foreach (Gun gun in starterGuns)
            AddGun(gun);

        currGunIndex = 0;
        UpdateGunVisual(); // update visuals
        uiController.UpdateGunHUD(guns[currGunIndex]); // update ui

        isFacingRight = true;
        canColorCycle = true;

    }

    private void Update() {

        // ground check
        isGrounded = Physics2D.OverlapCircle(leftFoot.position, groundCheckRadius, environmentMask) != null || Physics2D.OverlapCircle(rightFoot.position, groundCheckRadius, environmentMask) != null; // check both feet for ground check

        // movement
        horizontalInput = Input.GetAxisRaw("Horizontal");
        CheckFlip();

        // jumping
        if (Input.GetKeyDown(jumpKey) && isGrounded)
            Jump();

        if (Input.GetKeyUp(jumpKey) && rb.velocity.y > 0f) // if jump is let go in the air, player falls quicker
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

        // color switching
        if (Input.GetKeyDown(colorSwitchKey))
            CycleColor();

        // guns
        if (Input.GetMouseButton(0)) {

            StartCoroutine(guns[currGunIndex].Shoot(shootableMask, ShooterType.Player));
            uiController.UpdateGunHUD(guns[currGunIndex]);

        }

        // gun cycling
        if (Input.GetKeyDown(KeyCode.Alpha1))
            CycleToGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            CycleToGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            CycleToGun(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            CycleToGun(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            CycleToGun(4);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            CycleToGun(5);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            CycleToGun(6);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            CycleToGun(7);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            CycleToGun(8);

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
            CycleNextGun();
        else if (scrollInput < 0f)
            CyclePreviousGun();

        // gun reloading
        if (Input.GetKeyDown(reloadKey))
            StartCoroutine(guns[currGunIndex].Reload());

    }

    private void FixedUpdate() {

        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        // claim claimable tiles when player collides with them
        collision.transform.GetComponent<Claimable>()?.FadeColor(playerSprites[currSpriteIndex].GetMaterial().color);

    }

    private void OnApplicationQuit() {

        quitting = true;

    }

    private void OnBecameInvisible() {

        if (quitting) return; // to prevent error

        // player falls out of screen
        playerHealth.TakeDamage(playerHealth.GetHealth());

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

    private void CycleColor() {

        if (!canColorCycle) return;

        currSpriteIndex++;

        // loop sprite colors
        if (currSpriteIndex >= playerSprites.Length)
            currSpriteIndex = 0;

        spriteRenderer.sprite = playerSprites[currSpriteIndex].GetSprite();

        // if player is standing on something, claim it
        Collider2D leftCollider = Physics2D.OverlapCircle(leftFoot.position, groundCheckRadius, environmentMask);
        Collider2D rightCollider = Physics2D.OverlapCircle(rightFoot.position, groundCheckRadius, environmentMask);

        if (leftCollider != null)
            leftCollider.GetComponent<Claimable>()?.FadeColor(playerSprites[currSpriteIndex].GetMaterial().color);
        if (rightCollider != null)
            rightCollider.GetComponent<Claimable>()?.FadeColor(playerSprites[currSpriteIndex].GetMaterial().color);

        // start cooldown
        canColorCycle = false;
        Invoke("ColorCycleCooldownComplete", colorCycleCooldown);

    }

    private void ColorCycleCooldownComplete() {

        canColorCycle = true;

    }

    private void AddGun(Gun gun) {

        Gun newGun = Instantiate(gun, gunSlot); // add gun under gunSlot
        newGun.Initialize(collider);
        guns.Add(newGun);

    }

    private void CyclePreviousGun() {

        currGunIndex--;

        // cycle the guns in loop
        if (currGunIndex < 0)
            currGunIndex = guns.Count - 1;

        UpdateGunVisual(); // update visuals
        uiController.UpdateGunHUD(guns[currGunIndex]); // update ui

    }

    private void CycleToGun(int gunIndex) {

        if (gunIndex < 0 || gunIndex >= guns.Count)
            return;

        currGunIndex = gunIndex;

        UpdateGunVisual(); // update visuals
        uiController.UpdateGunHUD(guns[currGunIndex]); // update ui

    }

    private void CycleNextGun() {

        currGunIndex++;

        // cycle the guns in loop
        if (currGunIndex >= guns.Count)
            currGunIndex = 0;

        UpdateGunVisual(); // update visuals
        uiController.UpdateGunHUD(guns[currGunIndex]); // update ui

    }

    private void UpdateGunVisual() {

        // make all gun slot children invisible before cycling gun
        for (int i = 0; i < gunSlot.childCount; i++)
            gunSlot.GetChild(i).gameObject.SetActive(false);

        guns[currGunIndex].gameObject.SetActive(true); // make current gun visible

    }
}

[Serializable]
public class PlayerSprite {

    [SerializeField] private PlayerColor playerColor;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Material material;
    [SerializeField] private float multiplier;

    public PlayerColor GetPlayerColor() { return playerColor; }
    public Sprite GetSprite() { return sprite; }
    public Material GetMaterial() { return material; }

}

public enum PlayerColor {

    /*
    GUIDE:
        - Red: Attack Multiplier
        - Blue: Defense Multiplier
    */

    Red, Blue

}