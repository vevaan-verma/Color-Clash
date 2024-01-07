using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [Header("References")]
    private Rigidbody2D rb;

    [Header("Shooting")]
    private ShooterType shooterType;

    [Header("Movement")]
    [SerializeField] private float speed;
    private int damage;

    [Header("Impact")]
    [SerializeField] private GameObject impactEffect;

    [Header("Range")]
    private float maxRange;
    private Vector3 spawnPos;

    public void Initialize(ShooterType shooterType, int damage, Vector3 spawnPos, float maxRange, Collider2D shooterCollider) {

        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        this.shooterType = shooterType;
        this.damage = damage;
        this.spawnPos = spawnPos;
        this.maxRange = maxRange;

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooterCollider); // ignore collision with shooter

    }

    private void Update() {

        if (Vector3.Distance(spawnPos, transform.position) > maxRange) // check if bullet reached max range
            SelfDestruct();

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        bool? deathCaused = false; // to prevent impact effect when something dies (for better looking gfx)

        if (shooterType == ShooterType.Player)
            deathCaused = collision.transform.GetComponent<EnemyController>()?.TakeDamage(damage); // damage enemy if player is shooter
        else if (shooterType == ShooterType.Enemy)
            deathCaused = collision.transform.GetComponent<PlayerController>()?.TakeDamage(damage); // damage player if enemy is shooter

        if (deathCaused != null && !(bool) deathCaused)
            SelfDestruct();

    }

    private void SelfDestruct() {

        print("instantiate");
        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);

    }
}

public enum ShooterType {

    Player, Enemy

}
