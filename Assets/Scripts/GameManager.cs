using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("Levels")]
    [SerializeField] private Level[] levels;
    private AsyncOperation levelLoad;

    private void Awake() {

        DontDestroyOnLoad(gameObject);

    }

    public void StartLoadLevelAsync(int levelIndex) { // parameter is LEVEL index NOT scene index

        if (levels.Length == 0) return;

        levelLoad = SceneManager.LoadSceneAsync(levels[levelIndex].GetSceneBuildIndex());
        levelLoad.allowSceneActivation = false; // allow the loading screen to fully fade in before activating scene

    }

    public void FinishLevelLoad() {

        if (levelLoad == null) return;

        levelLoad.allowSceneActivation = true;

    }
}
