using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClaim : EntityClaim {

    [Header("Claim")]
    private EffectType effectType;
    private float multiplierAddition;

    public void Claim(EffectType effectType) {

        this.effectType = effectType;
        this.multiplierAddition = GetComponent<Claimable>().GetMultiplierAddition();
        levelManager.AddClaim(this);

    }

    private void OnDestroy() {

        levelManager.RemoveClaim(this);

    }

    public EffectType GetEffectType() { return effectType; }

    public float GetMultiplierAddition() { return multiplierAddition; }

}
