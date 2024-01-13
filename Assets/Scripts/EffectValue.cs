using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectValue {

    [SerializeField] private EffectType effectType;
    [SerializeField] private float effectMultiplier;

    public EffectType GetEffectType() { return effectType; }

    public float GetEffectMultiplier() { return effectMultiplier; }

}
