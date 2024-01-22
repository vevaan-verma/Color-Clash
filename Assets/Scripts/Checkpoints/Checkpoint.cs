using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public abstract class Checkpoint : MonoBehaviour {

    [Header("References")]
    [SerializeField] private SpriteRenderer arrowRenderer;
    private GameManager gameManager;
    private UIController uiController;
    private SpriteRenderer spriteRenderer;

    [Header("Mechanics")]
    [SerializeField] private MechanicType mechanicToUnlock;

    [Header("Color")]
    [SerializeField] private Color errorColor;
    [SerializeField] private float errorDisplayDuration;
    private Color startColor;
    private Color startArrowColor;

    [Header("Subtitles")]
    [SerializeField] private string[] subtitleTexts;
    [SerializeField] private float subtitleDisplayDuration;

    [Header("Destruction")]
    [SerializeField] private float destroyFadeDuration;

    protected void Start() {

        gameManager = FindObjectOfType<GameManager>();
        uiController = FindObjectOfType<UIController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startColor = spriteRenderer.color;
        startArrowColor = arrowRenderer.color;

        spriteRenderer.color = Color.clear; // set to clear for fade in
        arrowRenderer.color = Color.clear; // set to clear for fade in

        spriteRenderer.DOColor(startColor, destroyFadeDuration); // fade in
        arrowRenderer.DOColor(startArrowColor, destroyFadeDuration); // fade in arrow

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.CompareTag("Player")) { // collider is player

            if (CheckRequirements()) { // requirements are met

                if (subtitleTexts.Length > 1) // if there is more than one subtitle text, cycle through them
                    uiController.CycleSubtitleTexts(subtitleTexts, subtitleDisplayDuration); // start subtitle cycle
                else
                    uiController.SetSubtitleText(subtitleTexts[0]); // set subtitle text

                collision.GetComponent<PlayerController>().EnableMechanic(mechanicToUnlock); // unlock mechanic associated with checkpoint

                OnCheckpointDisable(); // allow subclasses to do something when checkpoint is disabled
                spriteRenderer.DOColor(Color.clear, destroyFadeDuration).OnComplete(() => gameObject.SetActive(false)); // fade out and disable
                arrowRenderer.DOColor(Color.clear, destroyFadeDuration); // fade out arrow

                gameManager.SetPlayerSpawn(transform.position); // update player spawn
                gameManager.UpdateCheckpoints(); // update checkpoints

            } else {

                spriteRenderer.DOColor(errorColor, errorDisplayDuration / 2f).OnComplete(() => spriteRenderer.DOColor(startColor, errorDisplayDuration / 2f)); // flash error color
                arrowRenderer.DOColor(errorColor, errorDisplayDuration / 2f).OnComplete(() => arrowRenderer.DOColor(startColor, errorDisplayDuration / 2f)); // flash error color

            }
        }
    }

    protected abstract void OnCheckpointDisable();

    protected abstract bool CheckRequirements();

}
