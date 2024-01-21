using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    [Header("References")]
    private GameCore gameCore;
    private MenuManager menuManager;

    [Header("Menu")]
    [SerializeField] private CanvasGroup menuScreen;
    [SerializeField] private float menuScreenFadeDuration;
    [SerializeField] private RectTransform menuButtonsLayout;
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button instructionsButton;
    [SerializeField] private Button quitButton;

    [Header("Instructions")]
    [SerializeField] private CanvasGroup instructionsScreen;
    [SerializeField] private float instructionsScreenFadeDuration;
    [SerializeField] private Button instructionsCloseButton;

    [Header("Loading")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingScreenFadeDuration;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private float loadingTextDisplayDuration;
    //private Coroutine loadingTextCoroutine;

    private void Start() {

        gameCore = FindObjectOfType<GameCore>();
        menuManager = FindObjectOfType<MenuManager>();

        // menu
        playButton.onClick.AddListener(Play);
        tutorialButton.onClick.AddListener(PlayTutorial);
        instructionsButton.onClick.AddListener(ShowInstructions);
        quitButton.onClick.AddListener(Quit);

        menuScreen.alpha = 0f; // reset alpha for fade
        menuScreen.gameObject.SetActive(true);
        menuScreen.DOFade(1f, menuScreenFadeDuration).SetEase(Ease.InCirc);

        LayoutRebuilder.ForceRebuildLayoutImmediate(menuScreen.GetComponent<RectTransform>()); // rebuild menu screen layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(menuButtonsLayout); // rebuild menu buttons layout

        // instructions
        instructionsCloseButton.onClick.AddListener(HideInstructions);

        instructionsScreen.gameObject.SetActive(false);
        instructionsScreen.alpha = 0f;

        // loading
        if (!menuManager.IsFirstLoadCompleted()) { // don't fade in loading screen on first load

            loadingScreen.alpha = 1f; // reset alpha for fade
            loadingScreen.gameObject.SetActive(true);
            loadingScreen.DOFade(0f, loadingScreenFadeDuration).OnComplete(() => loadingScreen.gameObject.SetActive(false)); // disable loading screen on complete & reset loading text

        }
    }

    private void Play() {

        menuScreen.gameObject.SetActive(false);

        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishLevelLoad());
        //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS
        gameCore.StartLoadLevelAsync(1); // load first level (level index 0 is tutorial)

    }

    private void PlayTutorial() {

        menuScreen.gameObject.SetActive(false);

        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishLevelLoad());
        //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS
        gameCore.StartLoadLevelAsync(0); // load tutorial (level index 0 is tutorial)

    }

    private void FinishLevelLoad() {

        //StopCoroutine(loadingTextCoroutine); // IMPORTANT TO PREVENT COROUTINE FROM CYCLING INFINITELY
        gameCore.FinishLevelLoad();

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

    private void ShowInstructions() {

        menuScreen.gameObject.SetActive(false);
        menuScreen.alpha = 0f; // reset alpha for fade

        instructionsScreen.gameObject.SetActive(true);
        instructionsScreen.DOFade(1f, instructionsScreenFadeDuration).SetEase(Ease.InCirc);

    }

    private void HideInstructions() {

        instructionsScreen.gameObject.SetActive(false);
        instructionsScreen.alpha = 0f;

        menuScreen.gameObject.SetActive(true);
        menuScreen.DOFade(1f, menuScreenFadeDuration).SetEase(Ease.InCirc); // alpha was reset when instructions were shown

    }

    private void Quit() {

        Application.Quit();

    }

    public void SetLoadingText(string text) {

        loadingText.text = text;

    }
}
