using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [Header("References")]
    private PlayerClaimManager claimManager;
    private CameraController cameraFollow;

    [Header("Constant Prefabs")]
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private GameManager gameManagerPrefab;
    [SerializeField] private LevelAudioManager audioManagerPrefab;

    [Header("Level")]
    [SerializeField] private Level level;

    [Header("Spawns")]
    [SerializeField] private Transform playerSpawn;

    [Header("Claims")]
    private List<Claimable> levelClaimables;
    private List<PlayerClaim> playerClaims;
    private List<PhantomClaim> enemyClaims;
    private int levelCurrClaimables; // for teleporter

    [Header("Teleporter")]
    [SerializeField] private Teleporter teleporter;

    private void Awake() {

        // add all claimables to list
        levelClaimables = new List<Claimable>();

        foreach (Claimable claimable in FindObjectsOfType<Claimable>())
            levelClaimables.Add(claimable);

        // destroy all game managers
        foreach (GameManager gameManager in FindObjectsOfType<GameManager>())
            Destroy(gameManager.gameObject);

        Instantiate(gameManagerPrefab); // instantiate game manager
        Instantiate(audioManagerPrefab).Initialize(); // instantiate audio manager

        // spawns
        SpawnPlayer();

        // claims
        claimManager = FindObjectOfType<PlayerClaimManager>();
        claimManager.transform.position = playerSpawn.position;

        playerClaims = new List<PlayerClaim>();
        enemyClaims = new List<PhantomClaim>();

    }

    private void Start() {

        SpawnEnemies(); // to allow enemy spawn class to run awake method first

    }

    private void SpawnPlayer() {

        // destroy existing players in scene
        foreach (PlayerController obj in FindObjectsOfType<PlayerController>())
            Destroy(obj.gameObject);

        cameraFollow = FindObjectOfType<CameraController>(); // IMPORTANT: SET THIS AFTER PLAYERS ARE DESTROYED
        cameraFollow.SetTarget(Instantiate(playerPrefab, playerSpawn.position + new Vector3(0f, playerPrefab.transform.localScale.y / 2f, 0f), Quaternion.identity).transform); // spawn player

    }

    private void SpawnEnemies() {

        // destroy existing enemies in scene
        foreach (PhantomController obj in FindObjectsOfType<PhantomController>())
            Destroy(obj.gameObject);

        foreach (PhantomSpawn enemySpawn in FindObjectsOfType<PhantomSpawn>())
            enemySpawn.SpawnEnemy(); // spawn enemy

    }

    public void AddClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Add(playerClaim);
            claimManager.AddClaimable(playerClaim.GetColor(), playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());
            levelCurrClaimables++;

            /* FOR ENDING GAME WHEN EVERYTHING IS CLAIMED
            CheckLevelClear(); // check if player has claimed all platforms
            */

            if (level.HasTeleporter() && levelClaimables.Contains(playerClaim.GetClaimable())) // to track how many required claims player has for teleporter
                teleporter.UpdateTeleporter();

        } else if (claim is PhantomClaim) {

            enemyClaims.Add((PhantomClaim) claim);

        }
    }

    public void RemoveClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Remove(playerClaim);
            claimManager.RemoveClaimable(playerClaim.GetColor(), playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());
            levelCurrClaimables--;

            if (level.HasTeleporter() && levelClaimables.Contains(playerClaim.GetClaimable())) // to track how many required claims player has for teleporter
                teleporter.UpdateTeleporter();

        } else if (claim is PhantomClaim) {

            enemyClaims.Remove((PhantomClaim) claim);

        }
    }

    public bool IsLevelCleared() {

        // make sure player has all claimables claimed
        bool found;

        foreach (Claimable claimable in levelClaimables) {

            found = false;

            for (int i = 0; i < playerClaims.Count; i++) {

                if (playerClaims[i].GetClaimable() == claimable) {

                    found = true;
                    break;

                }
            }

            if (!found)
                return false;

        }

        // make sure all phantoms are dead
        if (FindObjectsOfType<PhantomController>().Length != 0)
            return false;

        return true;

    }

    public int GetLevelSceneBuildIndex() { return level.GetSceneBuildIndex(); }

    public Vector3 GetPlayerSpawn() { return playerSpawn.position; }

    public AudioClip GetBackgroundMusic() { return level.GetBackgroundMusic(); }

    public List<PlayerClaim> GetPlayerClaims() { return playerClaims; }

    public List<PhantomClaim> GetEnemyClaims() { return enemyClaims; }

    public int GetLevelTotalClaimables() { return levelClaimables.Count; }

    public int GetLevelCurrentClaimables() { return levelCurrClaimables; }

    public bool LevelHasCode() { return level.HasCode(); }

}
