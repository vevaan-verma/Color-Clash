using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int currHealth;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathEffect;

    private void Start() {

        currHealth = maxHealth;

    }

    public void TakeDamage(int damage) {

        currHealth -= damage;

        if (currHealth <= 0f)
            Die();

    }

    private void Die() {

        Destroy(gameObject);
        Instantiate(deathEffect, transform.position, Quaternion.identity);

    }
}
