using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource soundEffectSource;
    private LevelManager levelManager;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip deathSound;

    // start function
    public void Initialize() {

        levelManager = FindObjectOfType<LevelManager>();

        musicSource.loop = true;

        musicSource.clip = levelManager.GetBackgroundMusic(); // play background music
        musicSource.Play();

    }

    public void PlaySound(SoundEffectType soundType) {

        switch (soundType) {

            case SoundEffectType.Footstep:

                if (!footstepSource.isPlaying)
                    footstepSource.PlayOneShot(footstepSound);
                break;

            case SoundEffectType.Land:

                soundEffectSource.PlayOneShot(landSound);
                break;

            case SoundEffectType.Death:

                soundEffectSource.PlayOneShot(deathSound);
                break;

        }
    }
}