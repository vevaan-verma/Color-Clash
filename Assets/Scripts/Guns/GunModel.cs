using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunModel : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private LineRenderer bulletTracer;
    [SerializeField] private GameObject impactEffect;

    public Transform GetMuzzle() { return muzzle; }
    public LineRenderer GetBulletTracer() { return bulletTracer; }
    public GameObject GetImpactEffect() { return impactEffect; }

}
