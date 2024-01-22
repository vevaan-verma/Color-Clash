using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCore : MonoBehaviour {

    [Header("Levels")]
    [SerializeField] private Level[] levels;
    [SerializeField] private int mainMenuBuildIndex;
    [SerializeField][Tooltip("Amount of scenes at beginning of build indexes that aren't levels (tutorial is a level)")] private int ignoredScenes;
    private AsyncOperation levelLoad;
    private AsyncOperation menuLoad;

    [Header("Gravity")]
    private Vector2 startGravity;

    [Header("Pausing")]
    private bool isPaused;

    [Header("Quitting")]
    private bool isQuitting;

    private void Start() {

        startGravity = Physics2D.gravity;

    }

    private void OnApplicationQuit() {

        isQuitting = true;
        DOTween.KillAll();

    }

    public void StartLoadMainMenuAsync() {

        isQuitting = true; // set quitting to true to prevent any coroutines from running
        menuLoad = SceneManager.LoadSceneAsync(mainMenuBuildIndex);
        menuLoad.allowSceneActivation = false; // allow the loading screen to fully fade in before activating scene

    }

    public void FinishMainMenuLoad() {

        if (menuLoad == null) return;

        menuLoad.allowSceneActivation = true;

    }

    public bool StartLoadLevelAsync(int levelIndex) { // PASSING -1 RELOADS CURRENT LEVEL) | returns if level was loaded

        if (levelIndex == -1) {

            isQuitting = true; // set quitting to true to prevent any coroutines from running
            levelLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex); // reload level
            levelLoad.allowSceneActivation = false; // allow the loading screen to fully fade in before activating scene
            return true;

        }

        if (levels.Length == 0 || levelIndex >= levels.Length + ignoredScenes) { // add two because main menu scene and tutorial level are not part of the levels array

            Debug.LogError("Level index " + levelIndex + " is invalid!");
            return false; // make sure levels array is not empty and level index is valid

        }

        isQuitting = true; // set quitting to true to prevent any coroutines from running
        levelLoad = SceneManager.LoadSceneAsync(levelIndex + ignoredScenes);
        levelLoad.allowSceneActivation = false; // allow the loading screen to fully fade in before activating scene
        return true;

    }

    public void FinishLevelLoad() {

        if (levelLoad == null) return;

        levelLoad.allowSceneActivation = true;

    }

    public void FlipGravity() {

        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -Physics2D.gravity.y); // flip gravity here because it is constant throughout scenes

    }

    public void ResetGravity() {

        Physics2D.gravity = startGravity;

    }

    public bool IsQuitting() { return isQuitting; }

    public bool IsPaused() { return isPaused; }

    public void PauseGame() {

        if (isPaused) return; // make sure game is not paused before pausing

        Time.timeScale = 0f; // pause time scale
        isPaused = true;

    }

    public void UnpauseGame() {

        if (!isPaused) return; // make sure game is paused before unpausing

        Time.timeScale = 1f; // set time scale to normal
        isPaused = false;

    }
}
