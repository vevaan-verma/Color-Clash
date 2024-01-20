using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("Levels")]
    [SerializeField] private Level[] levels;
    private AsyncOperation levelLoad;
    private AsyncOperation menuLoad;

    [Header("Gravity")]
    private Vector2 startGravity;

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

        menuLoad = SceneManager.LoadSceneAsync(0); // main menu has build index of 0
        menuLoad.allowSceneActivation = false; // allow the loading screen to fully fade in before activating scene

    }

    public void FinishMainMenuLoad() {

        if (menuLoad == null) return;

        menuLoad.allowSceneActivation = true;

    }

    public bool StartLoadLevelAsync(int levelIndex) { // parameter is LEVEL index NOT scene index (PASSING -1 RELOADS CURRENT LEVEL) | returns if level was loaded

        if (levelIndex == -1) {

            levelLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex); // reload level
            levelLoad.allowSceneActivation = false; // allow the loading screen to fully fade in before activating scene
            return true;

        }

        if (levels.Length == 0 || levelIndex >= levels.Length - 1) return false; // make sure levels array is not empty and level index is valid
        // TODO: don't show loading screen if level is invalid

        levelLoad = SceneManager.LoadSceneAsync(levels[levelIndex].GetSceneBuildIndex());
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

}
