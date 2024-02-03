using UnityEngine;

public class PlayerClaim : EntityClaim {

    [Header("Claim")]
    private Claimable claimable;
    private Color claimColor;
    private EffectType effectType;
    private float multiplierAddition;

    public void Claim(Claimable claimable, Color claimColor, EffectType effectType) {

        this.claimable = claimable;
        this.claimColor = claimColor;
        this.effectType = effectType;
        this.multiplierAddition = GetComponent<Claimable>().GetMultiplierAddition();
        gameManager.AddClaim(this);

    }

    private void OnDisable() {

        GetComponent<Claimable>().OnClaimDestroy(this); // trigger destroy event
        gameManager.RemoveClaim(this); // remove claim

    }

    public Claimable GetClaimable() { return claimable; }

    public Color GetColor() { return claimColor; }

    public EffectType GetEffectType() { return effectType; }

    public float GetMultiplierAddition() { return multiplierAddition; }

}
