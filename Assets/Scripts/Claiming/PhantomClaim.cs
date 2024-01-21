using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomClaim : EntityClaim {

    public void Claim() {

        gameManager.AddClaim(this);

    }

    private void OnDestroy() {

        GetComponent<Claimable>().OnClaimDestroy(this); // trigger destroy event
        gameManager.RemoveClaim(this); // remove claim

    }
}
