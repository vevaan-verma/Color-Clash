using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardedCheckpoint : Checkpoint {

    [Header("Phantoms")]
    [SerializeField] private PhantomSpawn[] phantomSpawns;

    protected override bool CheckRequirements() {

        foreach (PhantomSpawn phantomSpawn in phantomSpawns) // check if all phantoms are dead
            if (phantomSpawn.IsPhantomAlive()) // if any phantom is alive, return false
                return false;

        return true;

    }
}
