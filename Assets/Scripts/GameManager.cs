using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("Levels")]
    [SerializeField] private Level[] levels;
    private AsyncOperation levelLoad;
    private AsyncOperation menuLoad;

    public void StartLoadMainMenuAsync() {

        menuLoad = SceneManager.LoadSceneAsync(0); // main menu has build index of 0
        menuLoad.allowSceneActivation = false; // allow the loading screen to fully fade in before activating scene

    }

    public void FinishMainMenuLoad() {

        if (menuLoad == null) return;

        menuLoad.allowSceneActivation = true;

    }

    public void StartLoadLevelAsync(int levelIndex) { // parameter is LEVEL index NOT scene index (PASSING -1 RELOADS CURRENT LEVEL)

        if (levelIndex == -1) {

            levelLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex); // reload level
            levelLoad.allowSceneActivation = false; // allow the loading screen to fully fade in before activating scene
            return;

        }

        if (levels.Length == 0) return;

        levelLoad = SceneManager.LoadSceneAsync(levels[levelIndex].GetSceneBuildIndex());
        levelLoad.allowSceneActivation = false; // allow the loading screen to fully fade in before activating scene

    }

    public void FinishLevelLoad() {

        if (levelLoad == null) return;

        levelLoad.allowSceneActivation = true;

    }
}
