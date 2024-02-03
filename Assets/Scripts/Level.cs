using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject {

    [SerializeField] private new string name;
    [SerializeField] private int levelNumber;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float speedModifier;
    [SerializeField] private float jumpModifier;
    [SerializeField] private float gravityModifier;

    [SerializeField] private bool hasCode;
    [SerializeField] private bool hasTeleporter;
    [SerializeField] private bool isUnderwater;

    public string GetName() { return name; }

    public int GetLevelNumber() { return levelNumber; }

    public AudioClip GetBackgroundMusic() { return backgroundMusic; }

    public float GetSpeedModifier() { return speedModifier; }

    public float GetJumpModifier() { return jumpModifier; }

    public float GetGravityModifier() { return gravityModifier; }

    public bool HasCode() { return hasCode; }

    public bool HasTeleporter() { return hasTeleporter; }

    public bool IsUnderwater() { return isUnderwater; }

}
