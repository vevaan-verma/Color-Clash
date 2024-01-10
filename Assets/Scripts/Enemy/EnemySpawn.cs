using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    [Header("References")]
    private new BoxCollider2D collider;

    [Header("Spawn")]
    private bool canSpawn;
    private bool flipped;

    [Header("Patrol")]
    private EnemyPatrolRoute patrolRoute;

    [Header("Respawn")]
    [SerializeField] private bool respawnEnabled;
    [SerializeField] private float respawnWaitDuration; // TODO: move to enemy type class?
    private EnemyController enemyPrefab;

    private void Awake() {

        collider = GetComponent<BoxCollider2D>();

        flipped = transform.right.x < 0f;

        patrolRoute = GetComponentInChildren<EnemyPatrolRoute>();

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.CompareTag("Player")) // don't spawn enemy when player is standing on spawner
            canSpawn = false;

    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (collision.CompareTag("Player"))
            canSpawn = true;

    }

    public void SpawnEnemy(EnemyController enemyPrefab) {

        this.enemyPrefab = enemyPrefab;
        Instantiate(enemyPrefab, transform.position + new Vector3(0f, enemyPrefab.transform.localScale.y / 2f, 0f), Quaternion.identity).Initialize(this, flipped, patrolRoute.GetPatrolPoints());

    }

    public void OnEnemyDeath() {

        if (respawnEnabled)
            StartCoroutine(RespawnEnemy());

    }

    private IEnumerator RespawnEnemy() {

        yield return new WaitForSeconds(respawnWaitDuration);

        while (!canSpawn)
            yield return null;

        SpawnEnemy(enemyPrefab);

    }

    public bool IsFlipped() { return flipped; }

    public Transform[] GetPatrolPoints() { return patrolRoute.GetPatrolPoints(); }

    public bool IsRespawnEnabled() { return respawnEnabled; }

    public float GetRespawnWaitDuration() { return respawnWaitDuration; }

}
