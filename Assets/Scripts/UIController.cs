using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class UIController : MonoBehaviour {

    [Header("Cursor")]
    [SerializeField] private Transform crosshair;

    private void Start() {

        //Cursor.visible = false;

    }

    private void Update() {

        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        //crosshair.position = mousePosition;

    }
}
