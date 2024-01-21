using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("References")]
    private PlayerClaimManager claimManager;
    private PlayerController playerController;
    private PlayerGunManager gunManager;
    private PlayerHealthManager healthManager;
    private GameCore gameCore;
    private GameManager gameManager;
    private CodeManager codeManager;

    [Header("HUD")]
    [SerializeField] private CanvasGroup playerHUD;
    [SerializeField] private float playerHUDFadeDuration;

    [Header("Subtitles")]
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private float subtitleTypeDuration;
    private Coroutine subtitleCycleCoroutine;

    [Header("Claimables")]
    [SerializeField] private Transform claimablesInfoParent;
    [SerializeField] private ClaimableInfo claimablesInfoPrefab;
    private List<ClaimableInfo> claimablesInfo;

    [Header("Weapon HUD")]
    [SerializeField] private TMP_Text ammoText;

    [Header("Gun Cycle")]
    [SerializeField] private GameObject gunCycleParent;
    [SerializeField] private Image gunIconTop;
    [SerializeField] private Image gunIconMiddle;
    [SerializeField] private Image gunIconBottom;
    [SerializeField] private Sprite blankGunSprite;

    [Header("Health")]
    [SerializeField] private GameObject healthBarParent;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private float healthLerpDuration;
    [SerializeField] private Gradient healthGradient;
    private Coroutine healthLerpCoroutine;

    [Header("Code")]
    [SerializeField] private CanvasGroup codeHUD;
    [SerializeField] private float codeHUDFadeDuration;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private Button codeHUDCloseButton;
    private VaultController vaultController;

    [Header("Loading")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingScreenFadeDuration;
    [SerializeField] private TMP_Text loadingText;
    // [SerializeField] private float loadingTextDisplayDuration;

    [Header("Level Cleared")]
    [SerializeField] private CanvasGroup levelClearedScreen;
    [SerializeField] private RectTransform buttonsLayout;
    [SerializeField] private float levelClearedFadeDuration;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button nextLevelButton;

    public void Initialize() {

        claimManager = FindObjectOfType<PlayerClaimManager>();
        playerController = FindObjectOfType<PlayerController>();
        gunManager = FindObjectOfType<PlayerGunManager>();
        healthManager = FindObjectOfType<PlayerHealthManager>();
        gameCore = FindObjectOfType<GameCore>();
        gameManager = FindObjectOfType<GameManager>();

        // player HUD
        playerHUD.alpha = 0f; // reset alpha for fade
        playerHUD.gameObject.SetActive(true);
        playerHUD.DOFade(1f, playerHUDFadeDuration).SetEase(Ease.InCirc);

        claimablesInfoParent.gameObject.SetActive(true); // enabled by default
        gunCycleParent.SetActive(true); // enabled by default
        healthBarParent.SetActive(true); // enabled by default

        // set health slider values
        healthSlider.maxValue = healthManager.GetMaxHealth();
        healthSlider.value = healthSlider.maxValue;

        // claimable info
        claimablesInfo = new List<ClaimableInfo>();

        Dictionary<Color, int> claimables = claimManager.GetClaims();

        foreach (KeyValuePair<Color, int> pair in claimables) {

            claimablesInfo.Add(Instantiate(claimablesInfoPrefab, claimablesInfoParent)); // add to list
            claimablesInfo[claimablesInfo.Count - 1].UpdateInfo(pair.Key, pair.Value); // update info

        }

        // code
        if (gameManager is LevelManager && ((LevelManager) gameManager).LevelHasCode()) { // make sure level has code to avoid null errors  (make sure game manager is level manager)

            codeManager = FindObjectOfType<CodeManager>();
            vaultController = FindObjectOfType<VaultController>();

            codeHUDCloseButton.onClick.AddListener(CloseCodeHUD);

            codeHUD.gameObject.SetActive(false);
            codeHUD.alpha = 0f;

        }

        codeInput.onValueChanged.AddListener(InputChanged);

        // loading
        loadingScreen.alpha = 1f; // reset alpha for fade
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(0f, loadingScreenFadeDuration).OnComplete(() => loadingScreen.gameObject.SetActive(false)); // disable loading screen on complete & reset loading text

        // level cleared screen
        levelClearedScreen.gameObject.SetActive(false);
        levelClearedScreen.alpha = 0f;

        replayButton.onClick.AddListener(ReplayLevel);
        mainMenuButton.onClick.AddListener(LoadMainMenu);
        nextLevelButton.onClick.AddListener(LoadNextLevel);

    }

    public void UpdateClaimablesHUD() {

        Dictionary<Color, int> claimables = claimManager.GetClaims();

        foreach (ClaimableInfo info in claimablesInfo)
            info.UpdateInfo(claimables[info.GetColor()]); // update info

    }

    public void OpenCodeHUD() {

        playerController.DisableAllMechanics(); // disable all mechanics
        codeHUD.gameObject.SetActive(true);
        codeHUD.DOFade(1f, codeHUDFadeDuration); // disable loading screen on complete & reset loading text
        codeInput.text = ""; // clear input
        codeInput.ActivateInputField(); // select input field

    }

    public void CloseCodeHUD() {

        codeHUD.gameObject.SetActive(true);
        codeHUD.DOFade(0f, codeHUDFadeDuration).OnComplete(() => codeHUD.gameObject.SetActive(false)); // disable loading screen on complete & reset loading text
        playerController.EnableAllMechanics(); // enable all mechanics

    }

    private void InputChanged(string input) {

        if (codeManager.CheckCode(input)) {

            vaultController.Open();
            CloseCodeHUD();

        }
    }

    public void UpdateHealth() {

        if (healthLerpCoroutine != null)
            StopCoroutine(healthLerpCoroutine);

        healthLerpCoroutine = StartCoroutine(LerpHealth(healthManager.GetCurrentHealth(), healthLerpDuration));

    }

    private IEnumerator LerpHealth(float targetHealth, float duration) {

        float currentTime = 0f;
        float startHealth = healthSlider.value;

        while (currentTime < duration) {

            currentTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startHealth, targetHealth, currentTime / duration);
            sliderFill.color = healthGradient.Evaluate(healthSlider.normalizedValue); // normalizedValue returns the value between 0 and 1 (can't use DoTween here because of this line)
            yield return null;

        }

        healthSlider.value = targetHealth;
        healthLerpCoroutine = null;

    }

    public void UpdateGunHUD(Gun gun, int currGunIndex) {

        ammoText.text = gun.GetCurrentAmmo() + "/" + gun.GetMagazineSize();
        UpdateGunCycle(currGunIndex);

    }

    private void UpdateGunCycle(int currGunIndex) {

        List<Gun> guns = gunManager.GetGuns();

        if (guns.Count == 1) {

            gunIconTop.sprite = blankGunSprite; // top gun is blank
            gunIconMiddle.sprite = guns[0].GetIcon(); // middle gun is equipped gun
            gunIconBottom.sprite = blankGunSprite; // bottom gun is blank
            return;

        }

        if (currGunIndex == 0) { // equipped gun is first gun

            gunIconTop.sprite = guns[guns.Count - 1].GetIcon(); // top gun is last gun
            gunIconMiddle.sprite = guns[currGunIndex].GetIcon(); // middle gun is equipped gun
            gunIconBottom.sprite = guns[currGunIndex + 1].GetIcon(); // bottom gun is second gun

        } else if (currGunIndex == guns.Count - 1) {

            gunIconTop.sprite = guns[currGunIndex - 1].GetIcon(); // top gun is second last gun
            gunIconMiddle.sprite = guns[currGunIndex].GetIcon(); // middle gun is equipped gun
            gunIconBottom.sprite = guns[0].GetIcon(); // bottom gun is first gun

        } else {

            gunIconTop.sprite = guns[currGunIndex - 1].GetIcon(); // top gun is previous gun
            gunIconMiddle.sprite = guns[currGunIndex].GetIcon(); // middle gun is equipped gun
            gunIconBottom.sprite = guns[currGunIndex + 1].GetIcon(); // bottom gun is next gun

        }
    }

    public void OnLevelCleared() {

        // disable subtitles
        if (subtitleCycleCoroutine != null)
            StopCoroutine(subtitleCycleCoroutine);

        SetSubtitleText("");

        // disable player control
        playerController.DisableAllMechanics();

        levelClearedScreen.gameObject.SetActive(true);
        levelClearedScreen.DOFade(1f, levelClearedFadeDuration);
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelClearedScreen.GetComponent<RectTransform>()); // rebuild level cleared screen layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonsLayout); // rebuild button layout too

    }

    private void ReplayLevel() {

        levelClearedScreen.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishLevelLoad());
        //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS
        gameCore.StartLoadLevelAsync(-1); // pass -1 to reload level

    }

    private void LoadMainMenu() {

        levelClearedScreen.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishMainMenuLoad());
        //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS
        gameCore.StartLoadMainMenuAsync(); // load first level

    }

    private void FinishMainMenuLoad() {

        //StopCoroutine(loadingTextCoroutine); // IMPORTANT TO PREVENT COROUTINE FROM CYCLING INFINITELY
        gameCore.FinishMainMenuLoad();

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

    private void LoadNextLevel() {

        if (gameCore.StartLoadLevelAsync(gameManager.GetLevelIndex() + 1)) { // make sure level loads

            levelClearedScreen.gameObject.SetActive(false);
            loadingScreen.gameObject.SetActive(true);
            loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishLevelLoad());
            //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS

        }
    }

    public void SetSubtitleText(string text, bool stopCycle = true) {

        if (text == null || text == "") { // if text is empty, hide subtitle text

            HideSubtitleText();
            return;

        }

        if (stopCycle && subtitleCycleCoroutine != null) // stop cycle coroutine if it's running
            StopCoroutine(subtitleCycleCoroutine);

        subtitleText.gameObject.SetActive(true);
        DOVirtual.Int(0, text.Length, subtitleTypeDuration, (x) => subtitleText.text = text.Substring(0, x)).SetEase(Ease.Linear);

    }

    public void CycleSubtitleTexts(string[] subtitleTexts, float duration) {

        if (subtitleCycleCoroutine != null) // stop cycle coroutine if it's running
            StopCoroutine(subtitleCycleCoroutine);

        subtitleCycleCoroutine = StartCoroutine(StartCycleSubtitleTexts(subtitleTexts, duration));

    }

    private IEnumerator StartCycleSubtitleTexts(string[] subtitleTexts, float duration) {

        if (duration == 0f)
            Debug.LogWarning("Subtitle cycle duration is 0f! Please raise this value to increase performance.");

        while (true) {

            foreach (string subtitleText in subtitleTexts) {

                SetSubtitleText(subtitleText, false); // update subtitle text
                yield return new WaitForSeconds(duration); // wait for duration

            }
        }
    }

    public void HideSubtitleText() {

        subtitleText.gameObject.SetActive(false);

    }

    public void SetLoadingText(string text) {

        loadingText.text = text;

    }

    public void EnableClaimablesInfoHUD() { claimablesInfoParent.gameObject.SetActive(true); }

    public void DisableClaimablesInfoHUD() { claimablesInfoParent.gameObject.SetActive(false); }

    public void EnableGunCycleHUD() { gunCycleParent.SetActive(true); }

    public void DisableGunCycleHUD() { gunCycleParent.SetActive(false); }

    public void EnableHealthBarHUD() { healthBarParent.SetActive(true); }

    public void DisableHealthBarHUD() { healthBarParent.SetActive(false); }

}
