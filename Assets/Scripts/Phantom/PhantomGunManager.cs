using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhantomController))]
public class PhantomGunManager : MonoBehaviour {

    [Header("Shooting")]
    [SerializeField] private Transform gunSlot;
    [SerializeField] private LayerMask shootableMask; // just to avoid bullet collisions
    private Gun gun;

    public void SetGun(Gun gun) {

        // guns
        this.gun = Instantiate(gun, gunSlot);
        this.gun.Initialize(EntityType.Phantom, shootableMask, GetComponent<Collider2D>(), 0);

    }

    public void Shoot() {

        // gun shooting & reloading
        StartCoroutine(gun.Shoot());
        gun.InstantReload();

    }
}
