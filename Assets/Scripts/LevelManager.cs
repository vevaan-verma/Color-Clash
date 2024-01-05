using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;

    [Header("Level")]
    [SerializeField] private Transform spawn;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        playerController.transform.position = spawn.position;

    }

    public Vector3 GetSpawn() {

        return spawn.position;

    }
}
