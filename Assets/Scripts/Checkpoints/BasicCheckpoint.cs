using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCheckpoint : Checkpoint {

    protected override void OnCheckpointDisable() {

        // do nothing

    }

    protected override bool CheckRequirements() {

        return true; // no requirements for basic checkpoint

    }
}
