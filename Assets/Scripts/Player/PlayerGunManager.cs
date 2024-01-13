using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerGunManager : MonoBehaviour {

    [Header("References")]
    private PlayerColorManager colorManager;
    private PlayerEffectManager effectManager;
    private new Collider2D collider;
    private UIController uiController;

    [Header("Guns")]
    [SerializeField] private List<Gun> starterGuns; // DON'T USE GUNS FROM THIS, THEY AREN'T INSTANTIATED
    [SerializeField] private Transform gunSlot;
    [SerializeField] private LayerMask shootableMask; // just to avoid player and bullet collisions
    private List<Gun> guns; // contains the actual instantiated guns
    private int currGunIndex;

    [Header("Keybinds")]
    [SerializeField] private KeyCode reloadKey;

    private void Start() {

        colorManager = GetComponent<PlayerColorManager>();
        effectManager = GetComponent<PlayerEffectManager>();
        collider = GetComponent<Collider2D>();
        uiController = FindObjectOfType<UIController>();

        // guns
        guns = new List<Gun>();

        foreach (Gun gun in starterGuns)
            AddGun(gun);

        currGunIndex = 0;
        UpdateGunVisual(); // update visuals

    }

    private void Update() {

        // shooting
        if (Input.GetMouseButton(0)) {

            StartCoroutine(guns[currGunIndex].Shoot(EntityType.Player, shootableMask, colorManager.GetCurrentPlayerColor().GetEffectType() == EffectType.Damage ? effectManager.GetEffectMultiplier(EffectType.Damage) : 1f)); // if player has the damage color equipped, add multiplier
            uiController.UpdateGunHUD(guns[currGunIndex], currGunIndex);

        }

        // gun cycling
        if (Input.GetKeyDown(KeyCode.Alpha1))
            CycleToGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            CycleToGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            CycleToGun(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            CycleToGun(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            CycleToGun(4);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            CycleToGun(5);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            CycleToGun(6);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            CycleToGun(7);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            CycleToGun(8);

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
            CyclePreviousGun();
        else if (scrollInput < 0f)
            CycleNextGun();

        // gun reloading
        if (Input.GetKeyDown(reloadKey))
            StartCoroutine(guns[currGunIndex].Reload());

    }

    private void AddGun(Gun gun) {

        Gun newGun = Instantiate(gun, gunSlot); // add gun under gunSlot
        newGun.Initialize(collider, guns.Count); // index of new gun will be guns.Count
        guns.Add(newGun);

    }

    private void CyclePreviousGun() {

        if (guns[currGunIndex].IsReloading()) return; // deny swap if gun is reloading

        currGunIndex--;

        // cycle the guns in loop
        if (currGunIndex < 0)
            currGunIndex = guns.Count - 1;

        UpdateGunVisual(); // update visuals

    }

    private void CycleToGun(int gunIndex) {

        if (guns[currGunIndex].IsReloading()) return; // deny swap if gun is reloading

        if (gunIndex < 0 || gunIndex >= guns.Count)
            return;

        currGunIndex = gunIndex;

        UpdateGunVisual(); // update visuals

    }

    private void CycleNextGun() {

        if (guns[currGunIndex].IsReloading()) return; // deny swap if gun is reloading

        currGunIndex++;

        // cycle the guns in loop
        if (currGunIndex >= guns.Count)
            currGunIndex = 0;

        UpdateGunVisual(); // update visuals

    }

    private void UpdateGunVisual() {

        // make all gun slot children invisible before cycling gun
        for (int i = 0; i < gunSlot.childCount; i++)
            gunSlot.GetChild(i).gameObject.SetActive(false);

        guns[currGunIndex].gameObject.SetActive(true); // make current gun visible
        uiController.UpdateGunHUD(guns[currGunIndex], currGunIndex); // update ui

    }

    public List<Gun> GetGuns() { return guns; }

}
