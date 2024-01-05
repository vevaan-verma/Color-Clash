using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private float followSmoothing;
    private Vector3 offset;

    private void Start() {

        offset = transform.position - target.position;

    }

    private void LateUpdate() {

        transform.position = Vector3.Lerp(transform.position, target.position + offset, followSmoothing * Time.deltaTime);

    }
}
