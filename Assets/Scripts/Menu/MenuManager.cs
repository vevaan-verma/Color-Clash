using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    [Header("Loading")]
    private static bool firstLoadCompleted;

    [Header("Constant Prefabs")]
    [SerializeField] private GameManager gameManagerPrefab;
    [SerializeField] private MenuAudioManager audioManagerPrefab;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;

    private void Awake() {

        // destroy all game managers
        foreach (GameManager gameManager in FindObjectsOfType<GameManager>())
            Destroy(gameManager.gameObject);

        Instantiate(gameManagerPrefab); // instantiate game manager

        Instantiate(audioManagerPrefab).Initialize(); // instantiate audio manager

    }

    [RuntimeInitializeOnLoadMethod]
    private static void OnFirstLoad() { // only gets called on first load of game

        firstLoadCompleted = true;

    }

    public bool IsFirstLoadCompleted() { return firstLoadCompleted; }

    public AudioClip GetBackgroundMusic() { return backgroundMusic; }

}
