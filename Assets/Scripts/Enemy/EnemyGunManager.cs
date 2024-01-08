using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyGunManager : MonoBehaviour {

    [Header("Shooting")]
    [SerializeField] private Gun starterGun; // DON'T USE THIS GUN, IT ISN'T INSTANTIATED
    [SerializeField] private Transform gunSlot;
    [SerializeField] private LayerMask shootableMask; // just to avoid bullet collisions
    private Gun gun;

    private void Start() {

        // guns
        gun = Instantiate(starterGun, gunSlot);
        gun.Initialize(GetComponent<Collider2D>(), 0);

    }

    public void Shoot() {

        // gun shooting & reloading
        StartCoroutine(gun.Shoot(shootableMask, EntityType.Enemy));
        gun.InstantReload(EntityType.Enemy);

    }
}
