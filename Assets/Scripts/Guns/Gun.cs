using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Gun : ScriptableObject {

    [SerializeField] private GunModel gunModel;
    [SerializeField] private new string name;
    [SerializeField] private int damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float maxFireRange;
    [SerializeField] private float reloadTime;

    public GunModel GetModel() { return gunModel; }
    public string GetName() { return name; }
    public int GetDamage() { return damage; }
    public float GetFireRate() { return fireRate; }
    public float GetMaxFireRange() { return maxFireRange; }
    public float GetReloadTime() { return reloadTime; }

}
