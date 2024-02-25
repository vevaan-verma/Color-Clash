using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectData {

    [Header("Effect")]
    [SerializeField] private EffectType effectType;
    [SerializeField] private float effectMultiplier;
    [SerializeField] private Sprite effectIcon;

    public EffectType GetEffectType() { return effectType; }

    public float GetEffectMultiplier() { return effectMultiplier; }

    public void AddEffectMultiplier(float multiplier) { effectMultiplier += multiplier; }

    public void RemoveEffectMultiplier(float multiplier) { effectMultiplier -= multiplier; }

    public Sprite GetEffectIcon() { return effectIcon; }

}
