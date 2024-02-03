using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class BuoyancyOrb : MonoBehaviour {

    [Header("References")]
    private SpriteRenderer spriteRenderer;
    private new Collider2D collider;
    private GameManager gameManager;

    [Header("Buoyancy")]
    [SerializeField] private float buoyancyMultiplier;
    [SerializeField] private float destroyFadeDuration;

    private void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        gameManager = FindObjectOfType<GameManager>();

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.CompareTag("Player")) { // collider is player

            gameManager.SetGravity(Physics2D.gravity.y / buoyancyMultiplier);
            collider.enabled = false; // disable collider
            spriteRenderer.DOColor(Color.clear, destroyFadeDuration).OnComplete(() => Destroy(gameObject)); // fade out and destroy

        }
    }
}
