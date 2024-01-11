using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomSpawn : MonoBehaviour {

    [Header("Spawn")]
    [SerializeField] private PhantomController phantomPrefab;
    [SerializeField] private Gun gun;
    private bool flipped;

    [Header("Patrol")]
    private PhantomPatrolRoute patrolRoute;

    [Header("Respawn")]
    [SerializeField] private bool respawnEnabled;
    [SerializeField] private float respawnWaitDuration; // TODO: move to phantom type class?

    private void Awake() {

        flipped = transform.right.x < 0f;

        patrolRoute = GetComponentInChildren<PhantomPatrolRoute>();

    }

    public void SpawnEnemy() {

        Instantiate(phantomPrefab, transform.position + new Vector3(0f, phantomPrefab.transform.localScale.y / 2f, 0f), Quaternion.identity).Initialize(this, gun, flipped, patrolRoute.GetPatrolPoints());

    }

    public void OnEnemyDeath() {

        if (respawnEnabled)
            StartCoroutine(RespawnEnemy());

    }

    private IEnumerator RespawnEnemy() {

        yield return new WaitForSeconds(respawnWaitDuration);
        SpawnEnemy();

    }

    public bool IsFlipped() { return flipped; }

    public Transform[] GetPatrolPoints() { return patrolRoute.GetPatrolPoints(); }

    public bool IsRespawnEnabled() { return respawnEnabled; }

    public float GetRespawnWaitDuration() { return respawnWaitDuration; }

}
