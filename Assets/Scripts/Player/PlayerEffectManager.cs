using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerEffectManager : MonoBehaviour {

    [Header("Effects")]
    private Dictionary<EffectType, float> effectMultipliers;

    private void Start() {

        // effects
        effectMultipliers = new Dictionary<EffectType, float>();
        Array effects = Enum.GetValues(typeof(EffectType)); // get all effect type values

        // auto populate dictionary with all effect types
        foreach (EffectType effectType in effects)
            effectMultipliers.Add(effectType, 1f); // add with default value of 1 because that's the default multiplier

    }

    public void AddEffectMultiplier(EffectType effectType, float multiplier) {

        effectMultipliers[effectType] += multiplier;

    }

    public void RemoveEffectMultiplier(EffectType effectType, float multiplier) {

        effectMultipliers[effectType] -= multiplier;

    }

    public float GetEffectMultiplier(EffectType effectType) { return effectMultipliers[effectType]; }

}
