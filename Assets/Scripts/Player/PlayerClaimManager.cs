using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerClaimManager : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;
    private PlayerColorManager colorManager;
    private PlayerEffectManager effectManager;

    [Header("Claims")]
    [SerializeField] private float claimCheckRadius;
    private Dictionary<Color, int> claims;

    private void Awake() {

        playerController = GetComponent<PlayerController>();
        colorManager = GetComponent<PlayerColorManager>();
        effectManager = GetComponent<PlayerEffectManager>();

        // claimable info
        claims = new Dictionary<Color, int>();

        // auto populate dictionary with all claim colors
        foreach (PlayerColor playerColor in colorManager.GetPlayerColors())
            claims.Add(playerColor.GetClaimColor(), 0);

    }

    private void Update() {

        // if player is standing on something, claim it
        Collider2D leftCollider = Physics2D.OverlapCircle(playerController.GetLeftFoot().position, claimCheckRadius, playerController.GetEnvironmentMask());
        Collider2D rightCollider = Physics2D.OverlapCircle(playerController.GetRightFoot().position, claimCheckRadius, playerController.GetEnvironmentMask());

        if (leftCollider)
            leftCollider.GetComponent<Claimable>()?.Claim(EntityType.Player, colorManager.GetCurrentPlayerColor().GetClaimColor(), colorManager.GetCurrentPlayerColor().GetEffectType());
        if (rightCollider)
            rightCollider.GetComponent<Claimable>()?.Claim(EntityType.Player, colorManager.GetCurrentPlayerColor().GetClaimColor(), colorManager.GetCurrentPlayerColor().GetEffectType());

    }

    public void AddClaimable(Color claimColor, EffectType effectType, float addedMultiplier) {

        claims[claimColor]++; // add claimable
        effectManager.AddEffectMultiplier(effectType, addedMultiplier); // add effect multiplier to previous multiplier

    }

    public void RemoveClaimable(Color claimColor, EffectType effectType, float addedMultiplier) {

        claims[claimColor]--; // remove claimable
        effectManager.RemoveEffectMultiplier(effectType, addedMultiplier); // remove effect multiplier from previous multiplier

    }

    public Dictionary<Color, int> GetClaims() { return claims; }

}
