using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [Header("References")]
    private new Camera camera;

    [Header("Follow")]
    [SerializeField] private float followSmoothing;
    private Transform target;
    private Vector3 offset;

    [Header("Bounds")]
    [SerializeField] private BoxCollider2D mapBounds;
    private float xMin, yMin, xMax, yMax;
    private float camSize;
    private float camRatio;

    private void Start() {

        target = FindObjectOfType<PlayerController>().transform;
        camera = GetComponent<Camera>();

        offset = transform.position - target.position;

        xMin = mapBounds.bounds.min.x;
        yMin = mapBounds.bounds.min.y;
        xMax = mapBounds.bounds.max.x;
        yMax = mapBounds.bounds.max.y;

        camSize = camera.orthographicSize;
        camRatio = ((float) Screen.width / Screen.height) * camSize;

    }

    private void LateUpdate() {

        transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(target.position.x, xMin + camRatio, xMax - camRatio), Mathf.Clamp(target.position.y, yMin + camSize, yMax - camSize), 0f) + offset, followSmoothing * Time.deltaTime); // z value of vector3 should be zero because offset is being added after

    }

    public void SetTarget(Transform target) { this.target = target; }

}
