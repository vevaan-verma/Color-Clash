using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerColor {

    [SerializeField] private Color spriteColor;
    [SerializeField] private Color claimColor;
    [SerializeField] private EffectType effectType;
    [SerializeField] private float multiplier;

    public Color GetSpriteColor() { return spriteColor; }
    public Color GetClaimColor() { return claimColor; }
    public EffectType GetEffectType() { return effectType; }
    public float GetMultiplier() { return multiplier; }

}
