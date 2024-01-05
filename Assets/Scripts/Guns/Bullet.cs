using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [Header("References")]
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float speed;
    private int damage;

    [Header("Impact")]
    [SerializeField] private GameObject impactEffect;

    public void Initialize(int damage) {

        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        this.damage = damage;

    }

    private void OnCollisionEnter2D(Collision2D collision) {

        collision.transform.GetComponent<Enemy>()?.TakeDamage(damage);

        Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);

    }
}
