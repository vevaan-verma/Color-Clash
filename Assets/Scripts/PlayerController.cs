using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    private LevelManager levelManager;
    private GunController gunController;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private float horizontalInput;
    private bool isFacingRight;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;

    [Header("Guns")]
    [SerializeField] private Gun[] guns;
    [SerializeField] private Transform gunSlot;
    private int currGunIndex;
    private GunModel currGunModel;

    [Header("Ground Check")]
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private float groundCheckRadius;
    private bool isGrounded;

    [Header("Colors")]
    [SerializeField] private PlayerSprite[] playerSprites;
    private int currSpriteIndex;
    private SpriteRenderer spriteRenderer;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode colorSwitchKey;

    [Header("Quitting")]
    private bool quitting;

    private void Start() {

        levelManager = FindObjectOfType<LevelManager>();
        gunController = GetComponent<GunController>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currSpriteIndex = 0;
        spriteRenderer.sprite = playerSprites[currSpriteIndex].GetSprite();

        currGunIndex = 0;
        UpdateGunVisual();

        isFacingRight = true;

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
        if (Input.GetMouseButton(0))
            StartCoroutine(gunController.Shoot(guns[currGunIndex], currGunModel));

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
        Die();

    }

    private void CheckFlip() {

        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f) {

            transform.Rotate(0f, 180f, 0f);
            isFacingRight = !isFacingRight;

        }
    }

    private void Jump() {

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    }

    private void CycleColor() {

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

    }

    private void CyclePreviousGun() {

        currGunIndex--;

        // cycle the guns in loop
        if (currGunIndex < 0)
            currGunIndex = guns.Length - 1;

        UpdateGunVisual();

    }

    private void CycleToGun(int gunIndex) {

        if (gunIndex < 0 || gunIndex >= guns.Length)
            return;

        currGunIndex = gunIndex;

        UpdateGunVisual();

    }

    private void CycleNextGun() {

        currGunIndex++;

        // cycle the guns in loop
        if (currGunIndex >= guns.Length)
            currGunIndex = 0;

        UpdateGunVisual();

    }

    private void UpdateGunVisual() {

        // remove all gun slot children before adding new gun
        for (int i = 0; i < gunSlot.childCount; i++)
            Destroy(gunSlot.GetChild(i).gameObject);

        currGunModel = Instantiate(guns[currGunIndex].GetModel(), gunSlot); // add new gun as child

    }

    private void Die() {

        transform.position = levelManager.GetSpawn();

    }
}

[Serializable]
public class PlayerSprite {

    [SerializeField] private Sprite sprite;
    [SerializeField] private Material material;

    public Sprite GetSprite() { return sprite; }
    public Material GetMaterial() { return material; }

}
