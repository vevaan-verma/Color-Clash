using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerEffectManager : MonoBehaviour {

    [Header("References")]
    private UIController uiController;

    [Header("Effects")]
    [SerializeField] private List<EffectValue> effectValues;
    private Dictionary<EffectType, float> effectMultipliers;

    private void Start() {

        uiController = FindObjectOfType<UIController>();

        // effects
        effectMultipliers = new Dictionary<EffectType, float>();
        Array effects = Enum.GetValues(typeof(EffectType)); // get all effect type values

        bool added;

        // auto populate dictionary with all effect types
        foreach (EffectType effectType in effects) {

            added = false;

            // check for predefined multiplier
            for (int j = 0; j < effectValues.Count; j++) {

                // predefined multiplier is given
                if (effectValues[j].GetEffectType() == effectType) {

                    effectMultipliers.Add(effectType, effectValues[j].GetEffectMultiplier()); // add with multiplier
                    added = true;
                    break;

                }
            }

            if (!added)
                effectMultipliers.Add(effectType, 1f); // add with default value of 1 because no predefined multiplier was given

        }
    }

    public void AddEffectMultiplier(EffectType effectType, float multiplier) {

        effectMultipliers[effectType] += multiplier;
        uiController.UpdateClaimablesHUD(); // update ui

    }

    public void RemoveEffectMultiplier(EffectType effectType, float multiplier) {

        effectMultipliers[effectType] -= multiplier;
        uiController.UpdateClaimablesHUD(); // update ui

    }

    public float GetEffectMultiplier(EffectType effectType) { return effectMultipliers[effectType]; }

}
