using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudioManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private AudioSource musicSource;
    private MenuManager menuManager;

    // start function
    public void Initialize() {

        menuManager = FindObjectOfType<MenuManager>();

        musicSource.loop = true;

        PlayBackgroundMusic(menuManager.GetBackgroundMusic()); // play background music

    }

    private void PlayBackgroundMusic(AudioClip backgroundMusic) {

        musicSource.clip = backgroundMusic;
        musicSource.Play();

    }
}
