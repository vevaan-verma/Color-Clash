using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("References")]
    private PlayerHealthManager healthManager;
    private PlayerClaimManager claimManager;
    private PlayerGunManager gunManager;
    private GameManager gameManager;

    [Header("HUD")]
    [SerializeField] private CanvasGroup playerHUD;
    [SerializeField] private float playerHUDFadeDuration;

    [Header("Claimables")]
    [SerializeField] private Transform claimableInfoParent;
    [SerializeField] private ClaimableInfo claimableInfoPrefab;
    private List<ClaimableInfo> claimableInfos;

    [Header("Health")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private float healthLerpDuration;
    [SerializeField] private Gradient healthGradient;
    private Coroutine healthLerpCoroutine;

    [Header("Weapon HUD")]
    [SerializeField] private TMP_Text ammoText;

    [Header("Gun Cycle")]
    [SerializeField] private Image gunIconTop;
    [SerializeField] private Image gunIconMiddle;
    [SerializeField] private Image gunIconBottom;
    [SerializeField] private Sprite blankGunSprite;

    [Header("Loading")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingScreenFadeDuration;
    [SerializeField] private TMP_Text loadingText;
    // [SerializeField] private float loadingTextDisplayDuration;

    [Header("Level Cleared")]
    [SerializeField] private CanvasGroup levelClearedScreen;
    [SerializeField] private RectTransform buttonsLayout;
    [SerializeField] private float levelClearedFadeDuration;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button nextLevelButton;

    private void Awake() {

        gunManager = FindObjectOfType<PlayerGunManager>();
        claimManager = FindObjectOfType<PlayerClaimManager>();
        healthManager = FindObjectOfType<PlayerHealthManager>();
        gameManager = FindObjectOfType<GameManager>();

        // player HUD
        playerHUD.alpha = 0f; // reset alpha for fade
        playerHUD.gameObject.SetActive(true);
        playerHUD.DOFade(1f, playerHUDFadeDuration).SetEase(Ease.InCirc);

        // set health slider values
        healthSlider.maxValue = healthManager.GetMaxHealth();
        healthSlider.value = healthSlider.maxValue;

        //Cursor.visible = false;

        // claimable info
        claimableInfos = new List<ClaimableInfo>();

        Dictionary<Color, int> claimables = claimManager.GetClaims();

        foreach (KeyValuePair<Color, int> pair in claimables) {

            claimableInfos.Add(Instantiate(claimableInfoPrefab, claimableInfoParent)); // add to list
            claimableInfos[claimableInfos.Count - 1].UpdateInfo(pair.Key, pair.Value); // update info

        }

        // loading
        loadingScreen.alpha = 1f; // reset alpha for fade
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(0f, loadingScreenFadeDuration).OnComplete(() => loadingScreen.gameObject.SetActive(false)); // disable loading screen on complete & reset loading text

        // level cleared screen
        levelClearedScreen.gameObject.SetActive(false);
        levelClearedScreen.alpha = 0f;

        mainMenuButton.onClick.AddListener(LoadMainMenu);
        replayButton.onClick.AddListener(ReplayLevel);

    }

    public void UpdateClaimablesHUD() {

        Dictionary<Color, int> claimables = claimManager.GetClaims();

        foreach (ClaimableInfo info in claimableInfos)
            info.UpdateInfo(claimables[info.GetColor()]); // update info

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

        levelClearedScreen.gameObject.SetActive(true);
        levelClearedScreen.DOFade(1f, levelClearedFadeDuration);
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelClearedScreen.GetComponent<RectTransform>()); // rebuild level cleared screen layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonsLayout); // rebuild button layout too

    }

    private void LoadMainMenu() {

        levelClearedScreen.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishMainMenuLoad());
        //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS
        gameManager.StartLoadMainMenuAsync(); // load first level

    }

    private void FinishMainMenuLoad() {

        //StopCoroutine(loadingTextCoroutine); // IMPORTANT TO PREVENT COROUTINE FROM CYCLING INFINITELY
        gameManager.FinishMainMenuLoad();

    }

    private void ReplayLevel() {

        levelClearedScreen.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingScreenFadeDuration).SetEase(Ease.InCirc).OnComplete(() => FinishLevelLoad());
        //loadingTextCoroutine = StartCoroutine(UpdateLoadingText()); // REMEMBER TO STOP THIS COROUTINE BEFORE NEW SCENE LOADS
        gameManager.StartLoadLevelAsync(-1); // pass -1 to reload level

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

    public void SetLoadingText(string text) {

        loadingText.text = text;

    }
}
