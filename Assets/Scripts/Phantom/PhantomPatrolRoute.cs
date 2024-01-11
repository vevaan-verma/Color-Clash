using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomPatrolRoute : MonoBehaviour {

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;

    public Transform[] GetPatrolPoints() { return patrolPoints; }

}
