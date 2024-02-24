using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntityColor {

    [SerializeField] protected Color spriteColor;
    [SerializeField] protected Color claimColor;

    public Color GetSpriteColor() { return spriteColor; }

    public Color GetClaimColor() { return claimColor; }

}
