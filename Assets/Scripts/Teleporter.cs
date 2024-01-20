using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Teleporter : Interactable {

    [Header("References")]
    private GameManager gameManager;
    private LevelManager levelManager;
    private UIController uiController;

    [Header("Progress")]
    [SerializeField] private Slider teleporterProgressSlider;
    [SerializeField] private Slider teleporterUseSlider;

    [Header("Usage")]
    [SerializeField] private float progressLerpDuration;
    [SerializeField] private float useLerpDuration;

    private void Start() {

        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        uiController = FindObjectOfType<UIController>();

        // progress
        teleporterProgressSlider.maxValue = levelManager.GetLevelTotalClaimables();

    }

    private void Update() {

        if (levelManager.IsLevelCleared() && Vector3.Distance(transform.position, playerController.transform.position) <= interactDistance) { // interactable is in distance and can interact

            interactKeyIcon.SetActive(true); // show interact key icon

            if (Input.GetKeyDown(KeyCode.E))
                Interact();

        } else {

            interactKeyIcon.SetActive(false); // hide interact key icon

        }
    }

    protected override void Interact() {

        UseTeleporter();

    }

    public void UpdateTeleporter() {

        if (gameManager.IsQuitting()) return;

        teleporterProgressSlider.DOValue(levelManager.GetLevelCurrentClaimables(), progressLerpDuration);

    }

    private void UseTeleporter() {

        teleporterProgressSlider.value = teleporterProgressSlider.maxValue; // make sure progress slider is full
        uiController.OnLevelCleared();

    }
}
