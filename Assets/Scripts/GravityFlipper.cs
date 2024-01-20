using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFlipper : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;
    private GameManager gameManager;

    [Header("Rotation")]
    [SerializeField] private float rotationDuration;
    [SerializeField] private float rotationCooldown;
    private bool canRotate;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();

        canRotate = true;

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.transform.CompareTag("Player"))
            Flip();

    }

    private void Flip() {

        if (!canRotate) return;

        gameManager.FlipGravity();
        playerController.FlipPlayer(rotationDuration);
        canRotate = false;

        Invoke("ResetRotateCooldown", rotationCooldown);

    }

    private void ResetRotateCooldown() {

        canRotate = true;

    }
}
