using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityClaim : MonoBehaviour {

    [Header("References")]
    protected LevelManager levelManager;

    private void Awake() {

        levelManager = FindObjectOfType<LevelManager>();

    }
}
