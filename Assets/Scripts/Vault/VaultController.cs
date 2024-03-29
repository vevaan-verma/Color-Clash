using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultController : Interactable {

    [Header("References")]
    private UIController uiController;
    private SpriteRenderer spriteRenderer;
    private new Collider2D collider;

    [Header("Open & Close")]
    [SerializeField] private Sprite openSprite;
    private Sprite closedSprite;
    private bool isOpen;

    [Header("Reward")]
    [SerializeField] private GameObject reward;

    private void Start() {

        uiController = FindObjectOfType<UIController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();

        closedSprite = spriteRenderer.sprite; // closed sprite is current sprite

        reward.SetActive(false);

    }

    public override void Interact() {

        uiController.OpenCodeHUD();

    }

    public void Open() {

        if (isOpen) return;

        spriteRenderer.sprite = openSprite;
        reward.SetActive(true);
        collider.enabled = false; // disable collider to allow player to go into vault
        isOpen = true;

    }

    public void Close() {

        if (!isOpen) return;

        reward.SetActive(false);
        spriteRenderer.sprite = closedSprite;
        isOpen = false;

    }
}
