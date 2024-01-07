using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClaim : EntityClaim {

    public void Claim() {

        levelManager.AddClaim(this);

    }

    private void OnDestroy() {

        levelManager.RemoveClaim(this);

    }
}
