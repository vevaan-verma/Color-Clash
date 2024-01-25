using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Teleporter : Interactable {

    [Header("References")]
    private GameCore gameCore;
    private GameManager gameManager;
    private UIController uiController;

    [Header("Progress")]
    [SerializeField] private ProgressType progressType;
    [SerializeField] private Slider teleporterProgressSlider;

    [Header("Usage")]
    [SerializeField] private float progressLerpDuration;
    [SerializeField] private float useLerpDuration;

    private void Start() {

        gameCore = FindObjectOfType<GameCore>();
        gameManager = FindObjectOfType<GameManager>();
        uiController = FindObjectOfType<UIController>();

        // progress
        if (progressType == ProgressType.Checkpoints)
            teleporterProgressSlider.maxValue = gameManager.GetLevelTotalCheckpoints();
        else if (progressType == ProgressType.Claimables)
            teleporterProgressSlider.maxValue = gameManager.GetLevelTotalClaimables();

        teleporterProgressSlider.value = 0f;

    }

    public override void Interact() {

        if (gameManager.IsLevelObjectiveCompleted())
            UseTeleporter();

    }

    public void UpdateTeleporter() {

        if (gameCore.IsQuitting()) return;

        if (progressType == ProgressType.Checkpoints)
            teleporterProgressSlider.DOValue(gameManager.GetLevelCurrentCheckpoints(), progressLerpDuration);
        else if (progressType == ProgressType.Claimables)
            teleporterProgressSlider.DOValue(gameManager.GetLevelCurrentClaimables(), progressLerpDuration);

    }

    private void UseTeleporter() {

        teleporterProgressSlider.value = teleporterProgressSlider.maxValue; // make sure progress slider is full
        uiController.OnLevelCleared();

    }
}
