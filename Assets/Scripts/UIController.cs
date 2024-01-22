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
    private Animator animator;

    [Header("HUD")]
    [SerializeField] private CanvasGroup playerHUD;
    [SerializeField] private float playerHUDFadeDuration;

    [Header("Claimables")]
    [SerializeField] private CanvasGroup claimablesInfoParent;
    [SerializeField] private float claimablesInfoFadeDuration;
    [SerializeField] private ClaimableInfo claimablesInfoPrefab;
    private List<ClaimableInfo> claimablesInfo;

    [Header("Weapon HUD")]
    [SerializeField] private TMP_Text ammoText;

    [Header("Gun Cycle")]
    [SerializeField] private CanvasGroup gunCycleParent;
    [SerializeField] private float gunCycleFadeDuration;
    [SerializeField] private Image gunIconTop;
    [SerializeField] private Image gunIconMiddle;
    [SerializeField] private Image gunIconBottom;
    [SerializeField] private Sprite blankGunSprite;

    [Header("Health")]
    [SerializeField] private CanvasGroup healthBarParent;
    [SerializeField] private float healthBarFadeDuration;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private float healthLerpDuration;
    [SerializeField] private Gradient healthGradient;
    private Coroutine healthLerpCoroutine;

    [Header("Subtitles")]
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private float subtitleTypeDuration;
    private Coroutine subtitleCycleCoroutine;
    private bool subtitleVisible;

    [Header("Pause Menu")]
    [SerializeField] private CanvasGroup pauseMenu;
    [SerializeField] private Button pauseResumeButton;
    [SerializeField] private Button pauseRestartButton;
    [SerializeField] private Button pauseMainMenuButton;
    private Coroutine pauseMenuCoroutine;

    [Header("Code")]
    [SerializeField] private CanvasGroup codeHUD;
    [SerializeField] private float codeHUDFadeDuration;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private Button codeHUDCloseButton;
    private VaultController vaultController;
    private bool codeHUDVisible;

    [Header("Loading")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingScreenFadeDuration;
    [SerializeField] private TMP_Text loadingText;
    // [SerializeField] private float loadingTextDisplayDuration;
    private bool loadingScreenVisible;

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
        animator = GetComponent<Animator>();

        // player HUD
        playerHUD.alpha = 0f; // reset alpha for fade
        playerHUD.gameObject.SetActive(true);
        playerHUD.DOFade(1f, playerHUDFadeDuration).SetEase(Ease.InCirc);

        DisableClaimablesInfoHUD(); // disabled by default
        DisableGunCycleHUD(); // disabled by default
        DisableHealthBarHUD(); // disabled by default

        // set health slider values
        healthSlider.maxValue = healthManager.GetMaxHealth();
        healthSlider.value = healthSlider.maxValue;

        // claimable info
        claimablesInfo = new List<ClaimableInfo>();

        Dictionary<Color, int> claimables = claimManager.GetClaims();

        foreach (KeyValuePair<Color, int> pair in claimables) {

            claimablesInfo.Add(Instantiate(claimablesInfoPrefab, claimablesInfoParent.transform)); // add to list
            claimablesInfo[claimablesInfo.Count - 1].UpdateInfo(pair.Key, pair.Value); // update info

        }

        // pause menu
        pauseMenu.gameObject.SetActive(false);

        pauseResumeButton.onClick.AddListener(ClosePauseMenu); // close pause menu
        pauseRestartButton.onClick.AddListener(ReloadLevel); // reload level
        pauseMainMenuButton.onClick.AddListener(LoadMainMenu); // load main menu

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
        loadingScreen.DOFade(0f, loadingScreenFadeDuration).OnComplete(OnLoadComplete); // disable loading screen on complete
        loadingScreenVisible = true;

        // level cleared screen
        levelClearedScreen.gameObject.SetActive(false);
        levelClearedScreen.alpha = 0f;

        replayButton.onClick.AddListener(ReloadLevel);
        mainMenuButton.onClick.AddListener(LoadMainMenu);
        nextLevelButton.onClick.AddListener(LoadNextLevel);

    }

    private void OnLoadComplete() {

        loadingScreen.gameObject.SetActive(false);
        gameManager.Initialize(); // initialize game manager to enable mechanics & UI
        loadingScreenVisible = false;

    }

    public void UpdateClaimablesHUD() {

        Dictionary<Color, int> claimables = claimManager.GetClaims();

        foreach (ClaimableInfo info in claimablesInfo)
            info.UpdateInfo(claimables[info.GetColor()]); // update info

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

    public void TogglePause() {

        if (loadingScreenVisible || gameManager.IsLevelCleared()) return; // can't pause while loading or while level is completed

        CloseCodeHUD(); // close code HUD if it's open

        if (gameCore.IsPaused())
            ClosePauseMenu();
        else
            OpenPauseMenu();

    }

    private void OpenPauseMenu() {

        if (pauseMenuCoroutine != null) // stop coroutine if it's running
            StopCoroutine(pauseMenuCoroutine);

        if (subtitleVisible)
            subtitleText.gameObject.SetActive(false); // hide subtitle text if it's visible

        gameCore.PauseGame();
        playerController.DisableAllMechanics(); // disable all mechanics

        pauseMenu.gameObject.SetActive(true);
        pauseMenu.DOFade(1f, animator.GetCurrentAnimatorStateInfo(0).length).SetUpdate(true); // fade in pause menu with animation length (set update to true to ignore timescale)
        animator.SetTrigger("openPauseMenu");

    }

    private void ClosePauseMenu() {

        animator.SetTrigger("closePauseMenu"); // animation should disable pause menu on complete
        pauseMenu.DOFade(0f, animator.GetCurrentAnimatorStateInfo(0).length).SetUpdate(true); // fade out pause menu with animation length (set update to true to ignore timescale)

        if (subtitleVisible)
            subtitleText.gameObject.SetActive(true); // show subtitle text if it was visible before

        // give player mechanics back before disabling pause menu to make game feel more responsive
        gameCore.UnpauseGame();
        playerController.EnableAllMechanics(); // enable all mechanics

        pauseMenuCoroutine = StartCoroutine(DisablePauseMenu(animator.GetCurrentAnimatorStateInfo(0).length)); // disable pause menu after animation ends

    }

    private IEnumerator DisablePauseMenu(float waitDuration) { // for disabling pause menu after animation

        yield return new WaitForSecondsRealtime(waitDuration); // realtime to ignore timescale
        pauseMenu.gameObject.SetActive(false);

        pauseMenuCoroutine = null;

    }

    public void OpenCodeHUD() {

        if (codeHUDVisible) return; // don't open code HUD if it's already open

        playerController.DisableAllMechanics(); // disable all mechanics
        codeHUD.gameObject.SetActive(true);
        codeHUD.DOFade(1f, codeHUDFadeDuration); // disable loading screen on complete & reset loading text
        codeInput.text = ""; // clear input
        codeInput.ActivateInputField(); // select input field
        codeHUDVisible = true;

    }

    public void CloseCodeHUD() {

        if (!codeHUDVisible) return; // don't close code HUD if it's already closed

        codeHUD.gameObject.SetActive(true);
        codeHUD.DOFade(0f, codeHUDFadeDuration).OnComplete(() => codeHUD.gameObject.SetActive(false)); // disable loading screen on complete & reset loading text
        playerController.EnableAllMechanics(); // enable all mechanics
        codeHUDVisible = false;

    }

    private void InputChanged(string input) {

        if (codeManager.CheckCode(input)) {

            vaultController.Open();
            CloseCodeHUD();

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

    private void LoadMainMenu() {

        ClosePauseMenu(); // make sure game is unpaused before loading level

        levelClearedScreen.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishMainMenuLoad()); // set update to true to ignore timescale
        //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS
        gameCore.StartLoadMainMenuAsync(); // load first level

    }

    private void FinishMainMenuLoad() {

        //StopCoroutine(loadingTextCoroutine); // IMPORTANT TO PREVENT COROUTINE FROM CYCLING INFINITELY
        DOTween.KillAll(); // kill all tweens to prevent errors
        gameCore.FinishMainMenuLoad();

    }

    private void LoadNextLevel() {

        ClosePauseMenu(); // make sure game is unpaused before loading level

        if (gameCore.StartLoadLevelAsync(gameManager.GetLevelIndex() + 1)) { // make sure level loads

            levelClearedScreen.gameObject.SetActive(false);
            loadingScreen.gameObject.SetActive(true);
            loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishLevelLoad()).SetUpdate(true); // set update to true to ignore timescale
            //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS

        }
    }

    private void ReloadLevel() {

        ClosePauseMenu(); // make sure game is unpaused before loading level

        levelClearedScreen.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishLevelLoad()); // set update to true to ignore timescale
        //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS
        gameCore.StartLoadLevelAsync(-1); // pass -1 to reload level

    }

    private void FinishLevelLoad() {

        //StopCoroutine(loadingTextCoroutine); // IMPORTANT TO PREVENT COROUTINE FROM CYCLING INFINITELY
        DOTween.KillAll(); // kill all tweens to prevent errors
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

    public void SetSubtitleText(string text, bool stopCycle = true) {

        if (text == null || text == "") { // if text is empty, hide subtitle text

            HideSubtitleText();
            return;

        }

        if (stopCycle && subtitleCycleCoroutine != null) // stop cycle coroutine if it's running
            StopCoroutine(subtitleCycleCoroutine);

        subtitleText.gameObject.SetActive(true);
        DOVirtual.Int(0, text.Length, subtitleTypeDuration, (x) => subtitleText.text = text.Substring(0, x)).SetEase(Ease.Linear);
        subtitleVisible = true;

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

    public void HideSubtitleText() { // IMPORTANT: use this to disable subtitle text

        subtitleVisible = false;
        subtitleText.gameObject.SetActive(false);

    }

    public void SetLoadingText(string text) {

        loadingText.text = text;

    }

    public void EnableClaimablesInfoHUD() {

        claimablesInfoParent.gameObject.SetActive(true);
        claimablesInfoParent.DOFade(1f, claimablesInfoFadeDuration);

    }

    public void DisableClaimablesInfoHUD() {

        claimablesInfoParent.gameObject.SetActive(false);
        claimablesInfoParent.alpha = 0f; // reset alpha for fade

    }

    public void EnableGunCycleHUD() {

        gunCycleParent.gameObject.SetActive(true);
        gunCycleParent.DOFade(1f, gunCycleFadeDuration);

    }

    public void DisableGunCycleHUD() {

        gunCycleParent.gameObject.SetActive(false);
        gunCycleParent.alpha = 0f; // reset alpha for fade

    }

    public void EnableHealthBarHUD() {

        healthBarParent.gameObject.SetActive(true);
        healthBarParent.DOFade(1f, healthBarFadeDuration);

    }

    public void DisableHealthBarHUD() {

        healthBarParent.gameObject.SetActive(false);
        healthBarParent.alpha = 0f; // reset alpha for fade

    }
}
