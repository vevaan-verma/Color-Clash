using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [Header("References")]
    private new Camera camera;

    [Header("Follow")]
    [SerializeField] private float followSmoothing;
    private Transform player;
    private Vector3 offset;

    [Header("Bounds")]
    [SerializeField] private BoxCollider2D mapBounds;
    private float xMin, yMin, xMax, yMax;
    private float camSize;
    private float camRatio;

    private void Start() {

        player = FindObjectOfType<PlayerController>().transform;
        camera = GetComponent<Camera>();

        offset = transform.position - player.position;

        xMin = mapBounds.bounds.min.x;
        yMin = mapBounds.bounds.min.y;
        xMax = mapBounds.bounds.max.x;
        yMax = mapBounds.bounds.max.y;

        camSize = camera.orthographicSize;
        camRatio = (xMax + camSize) / 2f;

    }

    private void LateUpdate() {

        transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(player.position.x, xMin + camRatio, xMax - camRatio), Mathf.Clamp(player.position.y, yMin + camSize, yMax - camSize), transform.position.z) + offset, followSmoothing * Time.deltaTime);

    }
}
