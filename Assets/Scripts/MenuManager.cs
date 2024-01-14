using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    [Header("Constant Prefabs")]
    [SerializeField] private GameManager gameManagerPrefab;
    [SerializeField] private MenuAudioManager audioManagerPrefab;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;

    private void Start() {

        // destroy all game managers
        foreach (GameManager gameManager in FindObjectsOfType<GameManager>())
            Destroy(gameManager.gameObject);

        Instantiate(gameManagerPrefab); // instantiate game manager

        Instantiate(audioManagerPrefab).Initialize(); // instantiate audio manager

    }

    public AudioClip GetBackgroundMusic() { return backgroundMusic; }

}
