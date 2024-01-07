using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour {

    [Header("References")]
    private PlayerColorManager colorManager;
    private PlayerEffectManager effectManager;
    private PlayerGunManager gunManager;
    private LevelManager levelManager;
    private UIController uiController;
    private Rigidbody2D rb;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private float health;

    [Header("Regeneration")]
    [SerializeField] private int regenAmount;
    [SerializeField] private float regenWaitDuration;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathEffect;

    [Header("Quitting")]
    private bool quitting;

    private void Start() {

        colorManager = GetComponent<PlayerColorManager>();
        effectManager = GetComponent<PlayerEffectManager>();
        gunManager = GetComponent<PlayerGunManager>();
        levelManager = FindObjectOfType<LevelManager>();
        uiController = FindObjectOfType<UIController>();
        rb = GetComponent<Rigidbody2D>();

        SetHealth(maxHealth); // set health
        StartCoroutine(HandleRegeneration()); // start regeneration

    }

    private void OnApplicationQuit() {

        quitting = true;

    }

    private void OnBecameInvisible() {

        if (quitting) return; // to prevent error

        // player falls out of screen
        Die(); // kill player

    }

    // returns if player dies
    public bool TakeDamage(float damage) {

        RemoveHealth(damage * (colorManager.GetCurrentPlayerColor().GetEffectType() == EffectType.Defense ? (1f / effectManager.GetEffectMultiplier(EffectType.Defense)) : 1f));  // if player has the defense color equipped, add multiplier

        if (health <= 0f) {

            Die();
            return true;

        } else {

            return false;

        }
    }

    private void Die() {

        health = 0f;

        // reload all weapons
        foreach (Gun gun in gunManager.GetGuns())
            gun.InstantReload(EntityType.Player);

        rb.velocity = Vector2.zero; // reset velocity

        ParticleSystem.MainModule pm = Instantiate(deathEffect, transform.position, Quaternion.identity).main; // instantiate death effect where player died
        pm.startColor = colorManager.GetCurrentPlayerColor().GetSpriteColor(); // change particle color based on player color
        transform.position = levelManager.GetSpawn(); // respawn at level spawn
        SetHealth(maxHealth); // restore health

    }

    private void SetHealth(float health) {

        this.health = health;
        uiController.UpdateHealth(this.health);

    }

    private void AddHealth(float health) {

        this.health += health;
        uiController.UpdateHealth(this.health);

    }

    private void RemoveHealth(float health) {

        this.health -= health;
        uiController.UpdateHealth(this.health);

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

    public int GetMaxHealth() { return maxHealth; }

}
