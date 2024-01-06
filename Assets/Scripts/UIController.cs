using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class UIController : MonoBehaviour {

    [Header("Weapon HUD")]
    [SerializeField] private TMP_Text ammoText;

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

    public void UpdateGunHUD(Gun gun) {

        ammoText.text = gun.GetCurrentAmmo() + " / " + gun.GetMagazineSize();

    }

    public void SetAmmoReloadingText() {

        ammoText.text = "Reloading...";

    }
}
