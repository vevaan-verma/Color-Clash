using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealthManager : MonoBehaviour {

    [Header("References")]
    private PlayerColorManager colorManager;
    private PlayerEffectManager effectManager;
    private PlayerGunManager gunManager;
    private GameCore gameCore;
    private GameManager gameManager;
    private UIController uiController;
    private PlayerController playerController;
    private Rigidbody2D rb;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private float health;

    [Header("Regeneration")]
    [SerializeField] private int regenAmount;
    [SerializeField] private float regenWaitDuration;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathEffect;
    private bool isDead;

    [Header("Respawn")]
    [SerializeField] private float respawnTime;

    private void Start() {

        colorManager = GetComponent<PlayerColorManager>();
        effectManager = GetComponent<PlayerEffectManager>();
        gunManager = GetComponent<PlayerGunManager>();
        gameCore = FindObjectOfType<GameCore>();
        gameManager = FindObjectOfType<GameManager>();
        uiController = FindObjectOfType<UIController>();
        playerController = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        SetHealth(maxHealth); // set health
        StartCoroutine(HandleRegeneration()); // start regeneration

    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (gameCore.IsQuitting()) return; // to prevent errors

        // player falls out of map
        if (collision.CompareTag("Map"))
            StartCoroutine(Die()); // kill player

    }

    // returns if player dies
    public bool TakeDamage(float damage) {

        RemoveHealth(damage * (colorManager.GetCurrentPlayerColor().GetEffectType() == EffectType.Defense ? (1f / effectManager.GetEffectMultiplier(EffectType.Defense)) : 1f));  // if player has the defense color equipped, add multiplier

        if (health <= 0f) {

            StartCoroutine(Die()); // kill player
            return true;

        } else {

            return false;

        }
    }

    private IEnumerator Die() {

        isDead = true;

        health = 0f;

        playerController.ResetPlayer(); // reset player & camera (mainly for if player is rotated)

        // clear all player claims
        List<PlayerClaim> playerClaims = gameManager.GetPlayerClaims();

        foreach (PlayerClaim claim in playerClaims.ToList())
            Destroy(claim);

        // reload all weapons
        foreach (Gun gun in gunManager.GetGuns().ToList())
            gun.InstantReload();

        rb.velocity = Vector2.zero; // reset velocity

        ParticleSystem.MainModule pm = Instantiate(deathEffect, transform.position, Quaternion.identity).main; // instantiate death effect where player died
        pm.startColor = colorManager.GetCurrentPlayerColor().GetSpriteColor(); // change particle color based on player color

        yield return new WaitForSeconds(respawnTime);

        SetHealth(maxHealth); // restore health
        transform.position = gameManager.GetPlayerSpawn(); // respawn at level spawn
        uiController.UpdateClaimablesHUD(); // update ui

        isDead = false;

    }

    private void SetHealth(float health) {

        this.health = health;
        uiController.UpdateHealth();

    }

    private void AddHealth(float health) {

        this.health += health;
        uiController.UpdateHealth();

    }

    private void RemoveHealth(float health) {

        this.health -= health;
        uiController.UpdateHealth();

    }

    private IEnumerator HandleRegeneration() {

        while (true) {

            if (health < maxHealth) {

                AddHealth(regenAmount * (colorManager.GetCurrentPlayerColor().GetEffectType() == EffectType.Regeneration ? effectManager.GetEffectMultiplier(EffectType.Regeneration) : 1f)); // if player has the regeneration color equipped, add multiplier
                yield return new WaitForSeconds(regenWaitDuration);

            }

            yield return null;

        }
    }

    public float GetCurrentHealth() { return health; }

    public int GetMaxHealth() { return maxHealth; }

    public bool IsDead() { return isDead; }

}
