using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class GunData : ScriptableObject {

    [SerializeField] private new string name;
    [SerializeField] private Sprite icon;
    [SerializeField] private int damage;
    [SerializeField] private int magazineSize;
    [SerializeField] private float fireRate;
    [SerializeField] private float maxRange;
    [SerializeField] private float reloadTime;
    [SerializeField] private bool useRaycastShooting;

    public string GetName() { return name; }
    public Sprite GetIcon() { return icon; }
    public int GetDamage() { return damage; }
    public int GetMagazineSize() { return magazineSize; }
    public float GetFireRate() { return fireRate; }
    public float GetMaxRange() { return maxRange; }
    public float GetReloadTime() { return reloadTime; }
    public bool UsesRaycastShooting() { return useRaycastShooting; }

}
