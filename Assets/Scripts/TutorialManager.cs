using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialManager : GameManager {

    public override void Initialize() {

        // set mechanics defaults
        // only allow movement at beginning
        playerController.DisableAllMechanics();
        playerController.EnableMechanic(MechanicType.Movement);

        // set UI defaults
        uiController.DisableClaimablesInfoHUD();
        uiController.DisableGunCycleHUD();
        uiController.DisableHealthBarHUD();

    }

    public override void AddClaim(EntityClaim claim) {

        if (claim is PlayerClaim) {

            PlayerClaim playerClaim = (PlayerClaim) claim;
            playerClaims.Add(playerClaim);
            claimManager.AddClaimable(playerClaim.GetColor(), playerClaim.GetEffectType(), playerClaim.GetMultiplierAddition());
            levelCurrClaimables++;

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

        } else if (claim is PhantomClaim) {

            enemyClaims.Remove((PhantomClaim) claim);

        }
    }

    public override bool IsLevelObjectiveCompleted() {

        // make sure all checkpoints have been reached
        if (currCheckpointIndex == checkpoints.Length) // don't use length - 1 because checkpoint index is incremented after player reaches it
            return true;

        return false;

    }
}
