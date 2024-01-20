using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [Header("References")]
    protected PlayerController playerController;

    [Header("Interaction")]
    [SerializeField] protected float interactDistance;

    [Header("Cooldown")]
    [SerializeField] protected float interactCooldown;

    [Header("Icon")]
    [SerializeField] protected GameObject interactKeyIcon;

    protected void Awake() {

        playerController = FindObjectOfType<PlayerController>();
        interactKeyIcon.SetActive(false);

    }

    protected abstract void Interact();

}
