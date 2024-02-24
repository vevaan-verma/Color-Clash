using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomSpawn : MonoBehaviour {

    [Header("Spawn")]
    [SerializeField] private PhantomController phantomPrefab;
    [SerializeField] private Gun gun;
    private PhantomController currPhantom;
    private bool isFlipped;

    [Header("Patrol")]
    private PhantomPatrolRoute patrolRoute;

    [Header("Respawn")]
    [SerializeField] private bool respawnEnabled;
    [SerializeField] private float respawnWaitDuration;

    [Header("Claimable")]
    [SerializeField][Tooltip("Can be left null if no claimable platform is available nearby")] private Claimable claimablePlatform;

    private void Awake() {

        isFlipped = transform.right.x < 0f;

        patrolRoute = GetComponentInChildren<PhantomPatrolRoute>();

    }

    public void SpawnEnemy() {

        currPhantom = Instantiate(phantomPrefab, transform.position + new Vector3(0f, phantomPrefab.transform.localScale.y / 2f, 0f), isFlipped ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity);
        currPhantom.Initialize(this, gun, isFlipped, patrolRoute.GetPatrolPoints());

    }

    public void OnEnemyDeath() {

        if (respawnEnabled)
            StartCoroutine(RespawnEnemy());

    }

    private IEnumerator RespawnEnemy() {

        yield return new WaitForSeconds(respawnWaitDuration);

        if (claimablePlatform) // some spawns might not have a claimable platform
            while (claimablePlatform.GetClaimer() == EntityType.Player) // don't respawn enemy if claimed by player
                yield return null;

        if (respawnEnabled) // check again in case respawn was disabled while waiting
            SpawnEnemy();

    }

    public bool IsPhantomAlive() { return currPhantom != null; }

    public bool IsFlipped() { return isFlipped; }

    public Transform[] GetPatrolPoints() { return patrolRoute.GetPatrolPoints(); }

    public bool IsRespawnEnabled() { return respawnEnabled; }

    public void SetRespawnEnabled(bool respawnEnabled) { this.respawnEnabled = respawnEnabled; }

    public float GetRespawnWaitDuration() { return respawnWaitDuration; }

}
