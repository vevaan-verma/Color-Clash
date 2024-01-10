using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [Header("References")]
    private PlayerClaimManager claimManager;
    private CameraFollow cameraFollow;

    [Header("Constant Prefabs")]
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private AudioManager audioManagerPrefab;

    [Header("Level")]
    [SerializeField] private Level level;

    [Header("Spawns")]
    [SerializeField] private Transform playerSpawn;

    [Header("Claims")]
    private List<PlayerClaim> playerClaims;
    private List<EnemyClaim> enemyClaims;

    private void Awake() {

        Instantiate(audioManagerPrefab).Initialize();

        SpawnPlayer();

        claimManager = FindObjectOfType<PlayerClaimManager>();
        claimManager.transform.position = playerSpawn.position;

        playerClaims = new List<PlayerClaim>();
        enemyClaims = new List<EnemyClaim>();

    }

    private void Start() {

        SpawnEnemies(); // to allow enemy spawn class to run awake method first

    }

    private void SpawnPlayer() {

        // destroy existing players in scene
        foreach (PlayerController obj in FindObjectsOfType<PlayerController>())
            Destroy(obj.gameObject);

        cameraFollow = FindObjectOfType<CameraFollow>(); // IMPORTANT: SET THIS AFTER PLAYERS ARE DESTROYED
        cameraFollow.SetTarget(Instantiate(playerPrefab, playerSpawn.position + new Vector3(0f, playerPrefab.transform.localScale.y / 2f, 0f), Quaternion.identity).transform); // spawn player

    }

    private void SpawnEnemies() {

        // destroy existing enemies in scene
        foreach (EnemyController obj in FindObjectsOfType<EnemyController>())
            Destroy(obj.gameObject);

        foreach (EnemySpawn enemySpawn in FindObjectsOfType<EnemySpawn>())
            enemySpawn.SpawnEnemy(enemyPrefab); // spawn enemy & pass its patrol points

    }

    public void AddClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Add(playerClaim);
            claimManager.AddClaimable(playerClaim.GetColor(), playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());

        } else if (claim is EnemyClaim) {

            enemyClaims.Add((EnemyClaim) claim);

        }
    }

    public void RemoveClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Remove(playerClaim);
            claimManager.RemoveClaimable(playerClaim.GetColor(), playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());

        } else if (claim is EnemyClaim) {

            enemyClaims.Remove((EnemyClaim) claim);

        }
    }

    public Vector3 GetPlayerSpawn() { return playerSpawn.position; }

    public AudioClip GetBackgroundMusic() { return level.GetBackgroundMusic(); }

    public List<PlayerClaim> GetPlayerClaims() { return playerClaims; }

    public List<EnemyClaim> GetEnemyClaims() { return enemyClaims; }

}
