using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject {

    [SerializeField] private new string name;
    [SerializeField][Tooltip("Index of level in levels array")] private int levelIndex;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private bool hasCode;
    [SerializeField] private bool hasTeleporter;

    public string GetName() { return name; }

    public int GetLevelIndex() { return levelIndex; }

    public AudioClip GetBackgroundMusic() { return backgroundMusic; }

    public bool HasCode() { return hasCode; }

    public bool HasTeleporter() { return hasTeleporter; }

}
