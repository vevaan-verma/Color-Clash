using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject {

    [SerializeField] private new string name;
    [SerializeField] private int sceneBuildIndex;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private bool hasCode;
    [SerializeField] private bool hasTeleporter;
    [SerializeField] private bool isCameraRotated;

    public string GetName() { return name; }

    public int GetSceneBuildIndex() { return sceneBuildIndex; }

    public AudioClip GetBackgroundMusic() { return backgroundMusic; }

    public bool HasCode() { return hasCode; }

    public bool HasTeleporter() { return hasTeleporter; }

    public bool IsCameraRotated() { return isCameraRotated; }

}
