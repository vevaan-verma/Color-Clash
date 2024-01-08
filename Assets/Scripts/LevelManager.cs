using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [Header("References")]
    private PlayerClaimManager claimManager;

    [Header("Constant Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private AudioManager audioManagerPrefab;

    [Header("Level")]
    [SerializeField] private Level level;
    [SerializeField] private Transform spawn;

    [Header("Claims")]
    private List<PlayerClaim> playerClaims;
    private List<EnemyClaim> enemyClaims;

    private void Awake() {

        Instantiate(playerPrefab, spawn.position, Quaternion.identity);
        Instantiate(audioManagerPrefab).Initialize();

        claimManager = FindObjectOfType<PlayerClaimManager>();
        claimManager.transform.position = spawn.position;

        playerClaims = new List<PlayerClaim>();
        enemyClaims = new List<EnemyClaim>();

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

    public Vector3 GetSpawn() { return spawn.position; }

    public AudioClip GetBackgroundMusic() { return level.GetBackgroundMusic(); }

    public List<PlayerClaim> GetPlayerClaims() { return playerClaims; }

    public List<EnemyClaim> GetEnemyClaims() { return enemyClaims; }

}
