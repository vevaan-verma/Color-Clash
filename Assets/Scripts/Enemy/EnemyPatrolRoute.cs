using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolRoute : MonoBehaviour {

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;

    public Transform[] GetPatrolPoints() { return patrolPoints; }

}
