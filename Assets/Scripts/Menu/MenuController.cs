using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    [Header("References")]
    private GameManager gameManager;

    [Header("Menu")]
    [SerializeField] private CanvasGroup menuScreen;
    [SerializeField] private float menuScreenFadeDuration;
    [SerializeField] private Button playButton;
    [SerializeField] private Button instructionsButton;
    [SerializeField] private Button quitButton;

    [Header("Loading")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingScreenFadeDuration;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private float loadingTextDisplayDuration;
    //private Coroutine loadingTextCoroutine;

    private void Start() {

        gameManager = FindObjectOfType<GameManager>();

        // menu
        playButton.onClick.AddListener(Play);
        quitButton.onClick.AddListener(Quit);

        menuScreen.alpha = 0f; // reset alpha for fade
        menuScreen.gameObject.SetActive(true);
        menuScreen.DOFade(1f, menuScreenFadeDuration).SetEase(Ease.InCirc);

        // loading
        loadingScreen.alpha = 0f; // reset alpha for fade
        loadingScreen.gameObject.SetActive(false);

    }

    private void Play() {

        menuScreen.gameObject.SetActive(false);

        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishLevelLoad());
        //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS
        gameManager.StartLoadLevelAsync(0); // load first level

    }

    private void FinishLevelLoad() {

        //StopCoroutine(loadingTextCoroutine); // IMPORTANT TO PREVENT COROUTINE FROM CYCLING INFINITELY
        gameManager.FinishLevelLoad();

    }

    /*
    for adding ... after loading in a cycle (doesn't look good with current font)
    private IEnumerator UpdateLoadingText() {

        while (true) {

            for (int i = 0; i < 4; i++) {

                switch (i) {

                    case 0:

                        loadingText.text = "Loading";
                        break;

                    case 1:

                        loadingText.text = "Loading.";
                        break;

                    case 2:

                        loadingText.text = "Loading..";
                        break;

                    case 3:

                        loadingText.text = "Loading...";
                        break;

                }

                yield return new WaitForSeconds(loadingTextDisplayDuration);

            }

            yield return null;

        }
    }
    */

    private void Quit() {

        Application.Quit();

    }
}
