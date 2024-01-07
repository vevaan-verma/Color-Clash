using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;

    [Header("Constant Prefabs")]
    [SerializeField] private AudioManager audioManager;

    [Header("Level")]
    [SerializeField] private Level level;
    [SerializeField] private Transform spawn;

    [Header("Claims")]
    private List<PlayerClaim> playerClaims;
    private List<EnemyClaim> enemyClaims;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        playerController.transform.position = spawn.position;

        Instantiate(audioManager).Initialize();

        playerClaims = new List<PlayerClaim>();
        enemyClaims = new List<EnemyClaim>();

    }

    public void AddClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Add(playerClaim);
            playerController.AddEffect(playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());

        } else if (claim is EnemyClaim) {

            enemyClaims.Add((EnemyClaim) claim);

        }
    }

    public void RemoveClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Remove(playerClaim);
            playerController.RemoveEffect(playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());

        } else if (claim is EnemyClaim) {

            enemyClaims.Remove((EnemyClaim) claim);

        }
    }

    public Vector3 GetSpawn() { return spawn.position; }

    public AudioClip GetBackgroundMusic() { return level.GetBackgroundMusic(); }

    public List<PlayerClaim> GetPlayerClaims() { return playerClaims; }

    public List<EnemyClaim> GetEnemyClaims() { return enemyClaims; }

}
