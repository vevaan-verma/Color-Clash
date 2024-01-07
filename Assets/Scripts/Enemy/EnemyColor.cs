using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyColor {

    [SerializeField] private Color spriteColor;
    [SerializeField] private Color claimColor;

    public Color GetSpriteColor() { return spriteColor; }
    public Color GetClaimColor() { return claimColor; }

}
