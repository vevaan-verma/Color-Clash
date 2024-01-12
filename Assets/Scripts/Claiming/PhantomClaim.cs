using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomClaim : EntityClaim {

    public void Claim() {

        levelManager.AddClaim(this);

    }

    private void OnDestroy() {

        GetComponent<Claimable>().OnClaimDestroy(this); // trigger destroy event
        levelManager.RemoveClaim(this); // remove claim

    }
}
