
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Header("References")]
    private GameManager gameManager;
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

    [Header("Rotation")]
    private Tweener rotationTweener;

    private void Start() {

        gameManager = FindObjectOfType<GameManager>();
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

    public void RotateCamera(float duration, bool isRotated) {

        if (isRotated)
            rotationTweener = camera.transform.DORotate(Vector3.zero, duration, RotateMode.Fast);
        else
            rotationTweener = camera.transform.DORotate(new Vector3(0f, 0f, 180f), duration, RotateMode.Fast);

    }

    public void ResetCamera() {

        rotationTweener.Kill();
        gameManager.ResetGravity();
        camera.transform.rotation = Quaternion.identity;

    }

    public void SetTarget(Transform target) { this.target = target; }

}
