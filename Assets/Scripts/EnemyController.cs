using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    [Header("References")]
    private SpriteRenderer spriteRenderer;

    [Header("Shooting")]
    [SerializeField] private Gun starterGun; // DON'T USE THIS GUN, IT ISN'T INSTANTIATED
    [SerializeField] private Transform gunSlot;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask shootableMask; // just to avoid bullet collisions
    private Gun gun;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathEffect;

    private void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();

        health = maxHealth;

        gun = Instantiate(starterGun, gunSlot);
        gun.Initialize(GetComponent<Collider2D>());

    }

    private void Update() {

        StartCoroutine(gun.Shoot(shootableMask, ShooterType.Enemy));
        gun.InstantReload();

    }

    public void TakeDamage(int damage) {

        health -= damage;

        if (health <= 0f)
            Die();

    }

    private void Die() {

        Destroy(gameObject);
        ParticleSystem.MainModule pm = Instantiate(deathEffect, transform.position, Quaternion.identity).main;
        pm.startColor = spriteRenderer.color; // change particle color based on enemy color

    }
}
