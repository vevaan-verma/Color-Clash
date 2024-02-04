using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFlipper : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;
    private GameCore gameCore;

    [Header("Rotation")]
    [SerializeField] private float rotationDuration;
    [SerializeField] private float rotationCooldown;
    private bool canRotate;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        gameCore = FindObjectOfType<GameCore>();

        canRotate = true;

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.transform.CompareTag("Player"))
            Flip();

    }

    private void Flip() {

        if (!canRotate) return;

        gameCore.FlipGravity();
        playerController.GravityFlip(rotationDuration);
        canRotate = false;

        Invoke("ResetRotateCooldown", rotationCooldown);

    }

    private void ResetRotateCooldown() {

        canRotate = true;

    }
}
