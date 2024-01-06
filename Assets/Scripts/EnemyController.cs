using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    [Header("References")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [Header("Shooting")]
    [SerializeField] private Gun starterGun; // DON'T USE THIS GUN, IT ISN'T INSTANTIATED
    [SerializeField] private Transform gunSlot;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask shootableMask; // just to avoid bullet collisions
    private Gun gun;

    [Header("States")]
    private EnemyState enemyState;

    [Header("Patrol")]
    [SerializeField] private Transform pointA; // MUST BE POINT ON THE LEFT
    [SerializeField] private Transform pointB; // MUST BE POINT ON THE RIGHT
    [SerializeField] private float maxPointDistance;
    [SerializeField] private float destinationWaitDuration;
    private Transform currPoint;

    [Header("Chase")]
    private float targetDistance;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int health;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathEffect;

    private void Start() {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<PlayerController>().transform;

        health = maxHealth;

        gun = Instantiate(starterGun, gunSlot);
        gun.Initialize(GetComponent<Collider2D>());

        // states
        enemyState = EnemyState.Patrol;

        // patrolling
        currPoint = pointB;
        animator.SetBool("isRunning", true);

    }

    private void Update() {

        // gun shooting & reloading
        StartCoroutine(gun.Shoot(shootableMask, ShooterType.Enemy));
        gun.InstantReload();

        // chasing
        targetDistance = Vector2.Distance(transform.position, player.position);
        Vector2 targetDirection = player.position - transform.position;

        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

    }

    private IEnumerator Patrol() {

        while (true) {

            if (currPoint == pointB)
                rb.velocity = new Vector2(moveSpeed, 0f); // moves to the right
            else
                rb.velocity = new Vector2(-moveSpeed, 0f); // moves to the left

            if (Vector2.Distance(transform.position, currPoint.position) < maxPointDistance && currPoint == pointB) {

                // stop enemy and wait
                rb.velocity = Vector2.zero;
                yield return new WaitForSeconds(destinationWaitDuration);

                // redirect enemy patrol to point a and flip enemy
                currPoint = pointA;
                Flip();

            } else if (Vector2.Distance(transform.position, currPoint.position) < maxPointDistance && currPoint == pointA) {

                // stop enemy and wait
                rb.velocity = Vector2.zero;
                yield return new WaitForSeconds(destinationWaitDuration);

                // redirect enemy patrol to point b and flip enemy
                currPoint = pointB;
                Flip();

            }

            yield return null;

        }
    }

    private void Flip() {

        transform.Rotate(0f, 180f, 0f);

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

    private void OnDrawGizmos() {

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(pointA.position, maxPointDistance);
        Gizmos.DrawSphere(pointB.position, maxPointDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA.position, pointB.position);

    }
}

public enum EnemyState {

    Patrol, Chase

}