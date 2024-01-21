using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Claimable))]
public class EntityClaim : MonoBehaviour {

    [Header("References")]
    protected GameManager gameManager;

    private void Awake() {

        gameManager = FindObjectOfType<GameManager>();

    }
}
