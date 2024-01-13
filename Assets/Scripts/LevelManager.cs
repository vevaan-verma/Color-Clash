using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [Header("References")]
    private UIController uiController;
    private PlayerClaimManager claimManager;
    private CameraFollow cameraFollow;

    [Header("Constant Prefabs")]
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private AudioManager audioManagerPrefab;

    [Header("Level")]
    [SerializeField] private Level level;

    [Header("Spawns")]
    [SerializeField] private Transform playerSpawn;

    [Header("Claims")]
    private List<Claimable> levelClaimables;
    private List<PlayerClaim> playerClaims;
    private List<PhantomClaim> enemyClaims;

    [Header("Level Clear")]
    private bool levelCleared;

    private void Awake() {

        uiController = FindObjectOfType<UIController>();

        // add all claimables to list
        levelClaimables = new List<Claimable>();

        foreach (Claimable claimable in FindObjectsOfType<Claimable>())
            levelClaimables.Add(claimable);

        Instantiate(audioManagerPrefab).Initialize();

        SpawnPlayer();

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

        cameraFollow = FindObjectOfType<CameraFollow>(); // IMPORTANT: SET THIS AFTER PLAYERS ARE DESTROYED
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
            CheckLevelClear(); // check if player has claimed all platforms

        } else if (claim is PhantomClaim) {

            enemyClaims.Add((PhantomClaim) claim);

        }
    }

    public void RemoveClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Remove(playerClaim);
            claimManager.RemoveClaimable(playerClaim.GetColor(), playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());

        } else if (claim is PhantomClaim) {

            enemyClaims.Remove((PhantomClaim) claim);

        }
    }

    private bool CheckLevelClear() {

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

        levelCleared = true;
        uiController.OnLevelCleared();
        return true;

    }

    public Vector3 GetPlayerSpawn() { return playerSpawn.position; }

    public AudioClip GetBackgroundMusic() { return level.GetBackgroundMusic(); }

    public List<PlayerClaim> GetPlayerClaims() { return playerClaims; }

    public List<PhantomClaim> GetEnemyClaims() { return enemyClaims; }

}
