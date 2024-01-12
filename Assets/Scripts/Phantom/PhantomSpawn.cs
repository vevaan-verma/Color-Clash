using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomSpawn : MonoBehaviour {

    [Header("Spawn")]
    [SerializeField] private PhantomController phantomPrefab;
    [SerializeField] private Gun gun;
    private bool isFlipped;

    [Header("Patrol")]
    private PhantomPatrolRoute patrolRoute;

    [Header("Respawn")]
    [SerializeField] private bool respawnEnabled;
    [SerializeField] private float respawnWaitDuration; // TODO: move to phantom type class?

    private void Awake() {

        isFlipped = transform.right.x < 0f;

        patrolRoute = GetComponentInChildren<PhantomPatrolRoute>();

    }

    public void SpawnEnemy() {

        Instantiate(phantomPrefab, transform.position + new Vector3(0f, phantomPrefab.transform.localScale.y / 2f, 0f), isFlipped ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity).Initialize(this, gun, isFlipped, patrolRoute.GetPatrolPoints());

    }

    public void OnEnemyDeath() {

        if (respawnEnabled)
            StartCoroutine(RespawnEnemy());

    }

    private IEnumerator RespawnEnemy() {

        yield return new WaitForSeconds(respawnWaitDuration);
        SpawnEnemy();

    }

    public bool IsFlipped() { return isFlipped; }

    public Transform[] GetPatrolPoints() { return patrolRoute.GetPatrolPoints(); }

    public bool IsRespawnEnabled() { return respawnEnabled; }

    public float GetRespawnWaitDuration() { return respawnWaitDuration; }

}
