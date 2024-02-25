using UnityEngine;

public class LevelAudioManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource soundEffectSource;
    private GameManager gameManager;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip deathSound;

    // start function
    public void Initialize() {

        gameManager = FindObjectOfType<GameManager>();

        musicSource.loop = true;

        PlayBackgroundMusic(gameManager.GetBackgroundMusic()); // play background music

    }

    private void PlayBackgroundMusic(AudioClip backgroundMusic) {

        musicSource.clip = backgroundMusic;
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

    public void PlaySound(AudioClip audioClip) {

        soundEffectSource.PlayOneShot(audioClip);

    }
}