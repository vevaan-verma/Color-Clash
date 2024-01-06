using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    [Header("References")]
    private LevelManager levelManager;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathEffect;

    private void Start() {

        levelManager = FindObjectOfType<LevelManager>();

        health = maxHealth;

    }

    public void TakeDamage(int damage) {

        health -= damage;

        if (health <= 0f)
            Die();

    }

    private void Die() {

        Instantiate(deathEffect, transform.position, Quaternion.identity); // instantiate death effect where player died
        transform.position = levelManager.GetSpawn(); // respawn at level spawn
        health = maxHealth; // restore health

    }

    public int GetHealth() { return health; }

}
