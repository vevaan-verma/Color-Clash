using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(PhantomController))]
public class PhantomStateManager : MonoBehaviour {

    [Header("References")]
    private PhantomController phantomController;
    private PhantomGunManager gunManager;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private bool initialized;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [Header("Vision")]
    [SerializeField] private GameObject visionObj;
    [SerializeField] private Vector2 visionOffset;
    [SerializeField] private Vector2 visionSize;
    [SerializeField] private GameObject dangerIcon;
    private BoxCollider2D visionCollider;
    private bool playerInVision;

    [Header("States")]
    private PhantomState phantomState;
    private PhantomState lastPhantomState;

    [Header("Patrol")]
    [SerializeField] private float maxPointDistance;
    [SerializeField] private float destinationWaitDuration;
    private Transform[] patrolPoints;
    private int currPointIndex;

    [Header("Attack")]
    [SerializeField] private float firstShotDelay;

    public void Initialize(Transform[] patrolPoints) {

        this.patrolPoints = patrolPoints;
        initialized = true;

    }

    private IEnumerator Start() {

        phantomController = GetComponent<PhantomController>();
        gunManager = GetComponent<PhantomGunManager>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>().transform;

        // vision
        visionCollider = visionObj.AddComponent<BoxCollider2D>();

        visionCollider.offset = visionOffset;
        visionCollider.size = visionSize;
        visionCollider.isTrigger = true;

        dangerIcon.SetActive(false); // disable danger icon

        while (!initialized) // wait for initialization
            yield return null;

        // states
        phantomState = PhantomState.Patrol; // default state is patrol

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

        lastPhantomState = phantomState;

        if (playerInVision)
            phantomState = PhantomState.Attack;
        else
            phantomState = PhantomState.Patrol;

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.transform.CompareTag("Player")) { // collider is player

            playerInVision = true;

            // play danger animation
            dangerIcon.SetActive(true);
            animator.SetBool("playerInVision", true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (collision.transform.CompareTag("Player")) { // collider is player

            // stop danger animation
            animator.SetBool("playerInVision", false);
            dangerIcon.SetActive(false);

            playerInVision = false;

        }
    }

    private IEnumerator Patrol() {

        while (true) {

            if (phantomState == PhantomState.Patrol) {

                phantomController.CheckFlip();

                if (transform.position.x < patrolPoints[currPointIndex].position.x)
                    rb.velocity = new Vector2(moveSpeed, 0f); // move to the right
                else
                    rb.velocity = new Vector2(-moveSpeed, 0f); // moves to the left

                if (Vector2.Distance(transform.position, patrolPoints[currPointIndex].position) < maxPointDistance) {

                    // stop phantom and wait
                    rb.velocity = Vector2.zero;
                    animator.SetBool("isRunning", false);
                    yield return new WaitForSeconds(destinationWaitDuration);
                    animator.SetBool("isRunning", true);

                    // redirect phantom patrol to point a and flip phantom
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

            if (phantomState == PhantomState.Attack) {

                if (lastPhantomState != PhantomState.Attack) { // first frame of attack

                    animator.SetBool("isRunning", false);
                    rb.velocity = Vector2.zero; // stop movement

                    // phantom should look at player before shot delay
                    if ((player.position.x <= transform.position.x && phantomController.IsFacingRight()) // player is to the left of phantom
                        || (player.position.x > transform.position.x && !phantomController.IsFacingRight())) // player is to the right of phantom
                        phantomController.Flip();

                    yield return new WaitForSeconds(firstShotDelay); // wait for first shot delay

                }

                // make sure enemy is still attacking after delay
                if (phantomState == PhantomState.Attack) {

                    rb.velocity = Vector2.zero; // stop movement

                    gunManager.Shoot(); // shoot

                }
            }

            yield return null;

        }
    }

    private void OnDrawGizmosSelected() {

        if (Application.isPlaying || PrefabStageUtility.GetCurrentPrefabStage() != null) return; // don't draw gizmos in game or prefab mode

        // vision
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);

        if (transform.right.x > 0f)
            Gizmos.DrawCube((Vector2) transform.position + visionOffset, visionSize);
        else
            Gizmos.DrawCube((Vector2) transform.position - visionOffset, visionSize);

        // patrolling
        patrolPoints = null; // set to null to refresh every time

        if (patrolPoints == null && transform.parent.GetComponentInChildren<PhantomPatrolRoute>() != null) // check if phantom patrol route object exists
            patrolPoints = transform.parent.GetComponentInChildren<PhantomPatrolRoute>().GetPatrolPoints(); // get patrol points directly from phantom patrol route object instead of phantom spawn object because phantom spawn doesn't have a set reference yet

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
