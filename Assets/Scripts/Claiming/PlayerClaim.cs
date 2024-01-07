using UnityEngine;

public class PlayerClaim : EntityClaim {

    [Header("Claim")]
    private Color claimColor;
    private EffectType effectType;
    private float multiplierAddition;

    public void Claim(Color claimColor, EffectType effectType) {

        this.claimColor = claimColor;
        this.effectType = effectType;
        this.multiplierAddition = GetComponent<Claimable>().GetMultiplierAddition();
        levelManager.AddClaim(this);

    }

    private void OnDestroy() {

        levelManager.RemoveClaim(this);

    }

    public Color GetColor() { return claimColor; }

    public EffectType GetEffectType() { return effectType; }

    public float GetMultiplierAddition() { return multiplierAddition; }

}
