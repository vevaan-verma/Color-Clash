using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyStateManager : MonoBehaviour {

    [Header("References")]
    private EnemyController enemyController;
    private EnemyGunManager gunManager;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [Header("Vision")]
    [SerializeField] private GameObject visionObj;
    [SerializeField] private Vector2 visionOffset;
    [SerializeField] private Vector2 visionSize;
    private BoxCollider2D visionCollider;
    private bool playerInVision;

    [Header("States")]
    private EnemyState enemyState;
    private EnemyState lastEnemyState;

    [Header("Patrol")]
    [SerializeField] private float maxPointDistance;
    [SerializeField] private float destinationWaitDuration;
    private Transform[] patrolPoints;
    private int currPointIndex;

    [Header("Attack")]
    [SerializeField] private float firstShotDelay;

    private void Start() {

        enemyController = GetComponent<EnemyController>();
        gunManager = GetComponent<EnemyGunManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>().transform;

        // vision
        visionCollider = visionObj.AddComponent<BoxCollider2D>();

        visionCollider.offset = visionOffset;
        visionCollider.size = visionSize;
        visionCollider.isTrigger = true;

        // states
        enemyState = EnemyState.Patrol; // default state is patrol

        // patrolling
        if (patrolPoints.Length > 1) {

            currPointIndex = 1;
            animator.SetBool("isRunning", true);
            StartCoroutine(Patrol());

        }

        // attacking
        StartCoroutine(Attack());

    }

    private void Update() {

        lastEnemyState = enemyState;

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

    public void SetPatrolPoints(Transform[] patrolPoints) {

        this.patrolPoints = patrolPoints;

    }

    private IEnumerator Patrol() {

        while (true) {

            if (enemyState == EnemyState.Patrol) {

                enemyController.CheckFlip();

                if (transform.position.x < patrolPoints[currPointIndex].position.x)
                    rb.velocity = new Vector2(moveSpeed, 0f); // move to the right
                else
                    rb.velocity = new Vector2(-moveSpeed, 0f); // moves to the left

                if (Vector2.Distance(transform.position, patrolPoints[currPointIndex].position) < maxPointDistance) {

                    // stop enemy and wait
                    rb.velocity = Vector2.zero;
                    animator.SetBool("isRunning", false);
                    yield return new WaitForSeconds(destinationWaitDuration);
                    animator.SetBool("isRunning", true);

                    // redirect enemy patrol to point a and flip enemy
                    currPointIndex++;

                    if (currPointIndex >= patrolPoints.Length)
                        currPointIndex = 0;

                }
            }

            yield return null;

        }
    }

    private IEnumerator Attack() {

        while (true) {

            if (enemyState == EnemyState.Attack) {

                if (lastEnemyState != EnemyState.Attack) { // first frame of attack

                    animator.SetBool("isRunning", false);
                    rb.velocity = Vector2.zero; // stop movement
                    yield return new WaitForSeconds(firstShotDelay); // wait for first shot delay

                }

                if ((player.position.x <= transform.position.x && enemyController.IsFacingRight()) || (player.position.x > transform.position.x && !enemyController.IsFacingRight())) // player is to the left of enemy | player is to the right of enemy
                    enemyController.Flip();

                rb.velocity = Vector2.zero; // stop movement

                gunManager.Shoot(); // shoot

            }

            yield return null;

        }
    }

    private void OnDrawGizmosSelected() {

        // vision
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);

        if (transform.right.x > 0f)
            Gizmos.DrawCube((Vector2) transform.position + visionOffset, visionSize);
        else
            Gizmos.DrawCube((Vector2) transform.position - visionOffset, visionSize);

        // patrolling
        patrolPoints = null; // set to null to refresh every time

        if (patrolPoints == null && transform.parent.GetComponentInChildren<EnemyPatrolRoute>() != null) // check if enemy patrol route object exists
            patrolPoints = transform.parent.GetComponentInChildren<EnemyPatrolRoute>().GetPatrolPoints(); // get patrol points directly from enemy patrol route object instead of enemy spawn object because enemy spawn doesn't have a set reference yet

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);

        // draw patrol points
        foreach (Transform point in patrolPoints)
            Gizmos.DrawSphere(point.position, maxPointDistance);

        Gizmos.color = Color.red;

        // draw lines between points
        for (int i = 1; i < patrolPoints.Length; i++)
            Gizmos.DrawLine(patrolPoints[i - 1].position, patrolPoints[i].position);

    }
}
