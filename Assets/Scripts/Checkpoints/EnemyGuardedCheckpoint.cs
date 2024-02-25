using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardedCheckpoint : Checkpoint {

    [Header("Phantoms")]
    [SerializeField] private PhantomSpawn[] phantomSpawns;
    [SerializeField] private bool checkForDeath;

    protected override void OnCheckpointDisable() {

        foreach (PhantomSpawn phantomSpawn in phantomSpawns) // disable all phantoms from respawning
            phantomSpawn.SetRespawnEnabled(false);

    }

    protected override bool CheckRequirements() {

        if (!checkForDeath) return true; // if not checking for death, don't do anything

        foreach (PhantomSpawn phantomSpawn in phantomSpawns) // check if all phantoms are dead
            if (phantomSpawn.IsPhantomAlive()) // if any phantom is alive, return false
                return false;

        return true;

    }
}
