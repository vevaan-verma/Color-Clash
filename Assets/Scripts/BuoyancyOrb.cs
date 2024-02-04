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
    private GameCore gameCore;

    [Header("Buoyancy")]
    [SerializeField] private float buoyancyMultiplier;
    [SerializeField] private float destroyFadeDuration;

    private void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        gameCore = FindObjectOfType<GameCore>();

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.CompareTag("Player")) { // collider is player

            gameCore.ModifyGravity(1 / buoyancyMultiplier);
            collider.enabled = false; // disable collider
            spriteRenderer.DOColor(Color.clear, destroyFadeDuration).OnComplete(() => Destroy(gameObject)); // fade out and destroy

        }
    }
}
