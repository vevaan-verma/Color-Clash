using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelManager : GameManager {

    public override void Initialize() {

        playerController.EnableAllMechanics(); // enable all player controls

        // enable all UI
        uiController.EnableClaimablesInfoHUD();
        uiController.EnableGunCycleHUD();
        uiController.EnableHealthBarHUD();
    }

    public override void AddClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Add(playerClaim);
            claimManager.AddClaimable(playerClaim.GetColor(), playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());
            levelCurrClaimables++;

            /* FOR ENDING GAME WHEN EVERYTHING IS CLAIMED
            CheckLevelClear(); // check if player has claimed all platforms
            */

            // update teleporter because some track claimables
            if (level.HasTeleporter() && levelClaimables.Contains(playerClaim.GetClaimable()))
                teleporter.UpdateTeleporter();

        } else if (claim is PhantomClaim) {

            enemyClaims.Add((PhantomClaim) claim);

        }
    }

    public override void RemoveClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Remove(playerClaim);
            claimManager.RemoveClaimable(playerClaim.GetColor(), playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());
            levelCurrClaimables--;

            // update teleporter because some track claimables
            if (level.HasTeleporter() && levelClaimables.Contains(playerClaim.GetClaimable()))
                teleporter.UpdateTeleporter();

        } else if (claim is PhantomClaim) {

            enemyClaims.Remove((PhantomClaim) claim);

        }
    }

    public override bool IsLevelObjectiveCompleted() {

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
}
