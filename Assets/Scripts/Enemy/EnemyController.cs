using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Device;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

    [Header("References")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private bool isFacingRight;

    [Header("Shooting")]
    [SerializeField] private Gun starterGun; // DON'T USE THIS GUN, IT ISN'T INSTANTIATED
    [SerializeField] private Transform gunSlot;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask shootableMask; // just to avoid bullet collisions
    private Gun gun;

    [Header("Vision")]
    [SerializeField] private GameObject visionObj;
    [SerializeField] private Vector2 visionOffset;
    [SerializeField] private Vector2 visionSize;
    private BoxCollider2D visionCollider;
    private bool playerInVision;

    [Header("States")]
    private EnemyState enemyState;

    [Header("Patrol")]
    [SerializeField] private Transform pointA; // MUST BE POINT ON THE LEFT
    [SerializeField] private Transform pointB; // MUST BE POINT ON THE RIGHT
    [SerializeField] private float maxPointDistance;
    [SerializeField] private float destinationWaitDuration;
    private Transform currPoint;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private Transform healthCanvas;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private float healthLerpDuration;
    [SerializeField] private Gradient healthGradient;
    private Coroutine healthLerpCoroutine;
    private int health;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathEffect;

    /*
    IMPORTANT:
        - ENEMY MUST START FACING RIGHT
        - VISION OBJECT CANNOT BE ON ENEMY LAYER
    */

    private void Start() {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<PlayerController>().transform;
        visionCollider = visionObj.AddComponent<BoxCollider2D>();

        visionCollider.offset = visionOffset;
        visionCollider.size = visionSize;
        visionCollider.isTrigger = true;

        health = maxHealth;

        gun = Instantiate(starterGun, gunSlot);
        gun.Initialize(GetComponent<Collider2D>(), 0);

        // states
        enemyState = EnemyState.Patrol; // default state is patrol

        // patrolling
        currPoint = pointB;
        animator.SetBool("isRunning", true);
        StartCoroutine(Patrol());

        // attacking
        StartCoroutine(Attack());

        isFacingRight = true;

    }

    private void Update() {

        if (playerInVision)
            enemyState = EnemyState.Attack;
        else
            enemyState = EnemyState.Patrol;

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.transform.CompareTag("Player")) // collider is player
            playerInVision = true;

    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (collision.transform.CompareTag("Player")) // collider is player
            playerInVision = false;

    }

    private IEnumerator Patrol() {

        while (true) {

            if (enemyState == EnemyState.Patrol) {

                CheckFlip();

                if (currPoint == pointB)
                    rb.velocity = new Vector2(moveSpeed, 0f); // moves to the right
                else
                    rb.velocity = new Vector2(-moveSpeed, 0f); // moves to the left

                if (Vector2.Distance(transform.position, currPoint.position) < maxPointDistance && currPoint == pointB) {

                    // stop enemy and wait
                    rb.velocity = Vector2.zero;
                    animator.SetBool("isRunning", false);
                    yield return new WaitForSeconds(destinationWaitDuration);
                    animator.SetBool("isRunning", true);

                    // redirect enemy patrol to point a and flip enemy
                    currPoint = pointA;

                } else if (Vector2.Distance(transform.position, currPoint.position) < maxPointDistance && currPoint == pointA) {

                    // stop enemy and wait
                    rb.velocity = Vector2.zero;
                    animator.SetBool("isRunning", false);
                    yield return new WaitForSeconds(destinationWaitDuration);
                    animator.SetBool("isRunning", true);

                    // redirect enemy patrol to point b and flip enemy
                    currPoint = pointB;

                }
            }

            yield return null;

        }
    }

    private IEnumerator Attack() {

        while (true) {

            if (enemyState == EnemyState.Attack) {

                animator.SetBool("isRunning", false);

                if ((player.position.x <= transform.position.x && isFacingRight) || (player.position.x > transform.position.x && !isFacingRight)) // player is to the left of enemy | player is to the right of enemy
                    Flip();

                rb.velocity = Vector2.zero;

                // gun shooting & reloading
                StartCoroutine(gun.Shoot(shootableMask, ShooterType.Enemy));
                gun.InstantReload();

            }

            yield return null;

        }
    }

    private void CheckFlip() {

        if ((isFacingRight && rb.velocity.x < 0f) || (!isFacingRight && rb.velocity.x > 0f))
            Flip();

    }

    private void Flip() {

        transform.Rotate(0f, 180f, 0f);
        healthCanvas.Rotate(0f, 180f, 0f);
        isFacingRight = !isFacingRight; // breaks when there are errors

    }

    // returns if enemy dies
    public bool TakeDamage(int damage) {

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
        pm.startColor = spriteRenderer.color; // change particle color based on enemy color

    }

    public void UpdateHealth(int health) {

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
            sliderFill.color = healthGradient.Evaluate(healthSlider.normalizedValue); // normalizedValue returns the value between 0 and 1
            yield return null;

        }

        healthSlider.value = targetHealth;
        healthLerpCoroutine = null;

    }

    private void SetHealth(int health) {

        this.health = health;
        UpdateHealth(this.health);

    }

    private void AddHealth(int health) {

        this.health += health;
        UpdateHealth(this.health);

    }

    private void RemoveHealth(int health) {

        this.health -= health;
        UpdateHealth(this.health);

    }

    private void OnDrawGizmosSelected() {

        // vision
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);

        if (isFacingRight)
            Gizmos.DrawCube((Vector2) transform.position + visionOffset, visionSize);
        else
            Gizmos.DrawCube((Vector2) transform.position - visionOffset, visionSize);

        // patrolling
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(pointA.position, maxPointDistance);
        Gizmos.DrawSphere(pointB.position, maxPointDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA.position, pointB.position);

    }
}

public enum EnemyState {

    Patrol, Attack

}