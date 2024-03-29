using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhantomController))]
public class PhantomHealthManager : MonoBehaviour {

    [Header("References")]
    private PhantomController phantomController;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private Transform healthCanvas;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private float healthLerpDuration;
    [SerializeField] private Gradient healthGradient;
    private Coroutine healthLerpCoroutine;
    private float health;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathEffect;

    private void Start() {

        phantomController = GetComponent<PhantomController>();
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // set health & health slider values
        health = maxHealth;
        healthSlider.maxValue = health;
        healthSlider.value = healthSlider.maxValue;
        healthText.text = Mathf.CeilToInt(healthSlider.value) + ""; // health text is health rounded up

    }

    // returns if phantom dies
    public bool TakeDamage(float damage) {

        RemoveHealth(damage);

        if (health <= 0f) {

            Die();
            return true;

        } else {

            return false;

        }
    }

    private void Die() {

        Destroy(gameObject);
        ParticleSystem.MainModule pm = Instantiate(deathEffect, transform.position, Quaternion.identity).main;
        pm.startColor = spriteRenderer.color; // change particle color based on phantom color

        // clear all phantom claims
        List<PhantomClaim> phantomClaims = gameManager.GetEnemyClaims();

        foreach (PhantomClaim claim in phantomClaims.ToList()) // use ToList() to avoid InvalidOperationException
            Destroy(claim);

        phantomController.GetEnemySpawn().OnEnemyDeath(); // tell phantom spawn to respawn phantom if enabled

    }

    public void UpdateHealth(float health) {

        if (healthLerpCoroutine != null)
            StopCoroutine(healthLerpCoroutine);

        healthLerpCoroutine = StartCoroutine(LerpHealth(health, healthLerpDuration));

    }

    private IEnumerator LerpHealth(float targetHealth, float duration) {

        float currentTime = 0f;
        float startHealth = healthSlider.value;

        while (currentTime < duration) {

            currentTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startHealth, targetHealth, currentTime / duration);
            healthText.text = Mathf.CeilToInt(healthSlider.value) + ""; // health text is health rounded up
            sliderFill.color = healthGradient.Evaluate(healthSlider.normalizedValue); // normalizedValue returns the value between 0 and 1 (can't use DoTween here because of this line)
            yield return null;

        }

        healthSlider.value = targetHealth;
        healthLerpCoroutine = null;

    }

    private void SetHealth(float health) {

        this.health = health;
        UpdateHealth(this.health);

    }

    private void AddHealth(float health) {

        this.health += health;
        UpdateHealth(this.health);

    }

    private void RemoveHealth(float health) {

        this.health -= health;
        UpdateHealth(this.health);

    }

    public void FlipCanvas() {

        healthCanvas.Rotate(0f, 180f, 0f);

    }
}
