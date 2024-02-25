using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerEffectManager : MonoBehaviour {

    [Header("References")]
    private UIController uiController;

    [Header("Effects")]
    [SerializeField] private List<EffectData> effectMultipliers; // declare all default effect multipliers

    private void Start() {

        uiController = FindObjectOfType<UIController>();

    }

    public float GetEffectMultiplier(EffectType effectType) {

        foreach (EffectData effectData in effectMultipliers)
            if (effectData.GetEffectType() == effectType)
                return effectData.GetEffectMultiplier();

        return 0f;

    }

    public void AddEffectMultiplier(EffectType effectType, float multiplier) {

        foreach (EffectData effectData in effectMultipliers) {

            if (effectData.GetEffectType() == effectType) {

                effectData.AddEffectMultiplier(multiplier);
                uiController.UpdateClaimablesHUD(); // update ui
                return;

            }
        }
    }

    public void RemoveEffectMultiplier(EffectType effectType, float multiplier) {

        foreach (EffectData effectData in effectMultipliers) {

            if (effectData.GetEffectType() == effectType) {

                effectData.RemoveEffectMultiplier(multiplier);
                uiController.UpdateClaimablesHUD(); // update ui
                return;

            }
        }
    }

    public List<EffectData> GetEffectMultipliers() { return effectMultipliers; }

}
