using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractManager : MonoBehaviour {

    [Header("Interacting")]
    [SerializeField] private float interactRadius;
    [SerializeField] private float interactCooldown;
    [SerializeField] private LayerMask interactableMask;
    private List<Interactable> detectedInteractables; // to remove interact key icon when player is not in range

    [Header("Keybinds")]
    [SerializeField] private KeyCode interactKey;

    private void Start() {

        detectedInteractables = new List<Interactable>();

    }

    private void Update() {

        Interactable interactable = Physics2D.OverlapCircle(transform.position, interactRadius, interactableMask)?.GetComponent<Interactable>(); // get interactable

        if (interactable != null) { // if interactable is not null

            // show interact key icon and add to detected interactables list
            interactable.ShowInteractKeyIcon();
            detectedInteractables.Add(interactable);

            if (Input.GetKeyDown(interactKey)) // check for interact key press
                interactable.Interact();

        }

        foreach (Interactable detected in detectedInteractables)
            if (detected != interactable) detected.HideInteractKeyIcon(); // hide interact key icon for all detected interactables except the current interactable

    }
}
