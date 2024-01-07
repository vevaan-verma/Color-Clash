using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerColor : EntityColor {

    [SerializeField] private EffectType effectType;

    public EffectType GetEffectType() { return effectType; }

}
