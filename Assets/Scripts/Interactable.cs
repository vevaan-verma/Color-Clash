using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [Header("References")]
    protected PlayerController playerController;

    [Header("Icon")]
    [SerializeField] protected SpriteRenderer interactKeyIcon;
    [SerializeField] protected float iconFadeDuration;
    private Color startColor;
    // for tracking if icon is visible
    private bool isVisible;
    private Tweener keyIconTweenIn;
    private Tweener keyIconTweenOut;

    protected void Awake() {

        playerController = FindObjectOfType<PlayerController>();

        startColor = interactKeyIcon.color;
        interactKeyIcon.gameObject.SetActive(false);
        interactKeyIcon.color = Color.clear; // set to clear for fade in

    }

    public abstract void Interact();

    public void ShowInteractKeyIcon() {

        if (isVisible) return;

        if (keyIconTweenOut != null && keyIconTweenOut.IsActive()) keyIconTweenOut.Kill();

        isVisible = true;
        interactKeyIcon.gameObject.SetActive(true);
        interactKeyIcon.DOColor(startColor, iconFadeDuration);

    }

    public void HideInteractKeyIcon() {

        if (!isVisible) return;

        if (keyIconTweenIn != null && keyIconTweenIn.IsActive()) keyIconTweenIn.Kill();

        isVisible = false;
        keyIconTweenOut = interactKeyIcon.DOColor(Color.clear, iconFadeDuration).OnComplete(() => interactKeyIcon.gameObject.SetActive(false));

    }

    public void SetPlayerController(PlayerController playerController) { this.playerController = playerController; }

}
