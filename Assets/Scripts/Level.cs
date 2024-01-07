using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject {

    [SerializeField] private new string name;
    [SerializeField] private Object scene;
    [SerializeField] private AudioClip backgroundMusic;

    public string GetName() { return name; }

    public Object GetScene() { return scene; }

    public AudioClip GetBackgroundMusic() { return backgroundMusic; }

}
