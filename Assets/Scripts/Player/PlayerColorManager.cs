using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerColorManager : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;
    private UIController uiController;

    [Header("Color Cycling")]
    [SerializeField] private PlayerColor[] playerColors;
    [SerializeField] private float colorCycleCooldown;
    private int currColorIndex;
    private SpriteRenderer spriteRenderer;
    private bool canColorCycle;

    [Header("Keybinds")]
    [SerializeField] private KeyCode colorSwitchKey;

    private void Start() {

        playerController = GetComponent<PlayerController>();
        uiController = FindObjectOfType<UIController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currColorIndex = 0;
        spriteRenderer.color = playerColors[currColorIndex].GetSpriteColor();

        uiController.UpdateEffectText(playerColors[currColorIndex]); // update effect text
        uiController.UpdateClaimablesHUD(); // update claimables HUD (for selected indicator)

        canColorCycle = true;

    }

    private void Update() {

        // color switching
        if (playerController.IsMechanicEnabled(MechanicType.ColorCycling)) // don't return if false to allow for more code to be added to this method later
            if (Input.GetKeyDown(colorSwitchKey))
                CycleColor();

    }

    private void CycleColor() {

        if (!canColorCycle) return;

        currColorIndex++;

        // loop sprite colors
        if (currColorIndex >= playerColors.Length)
            currColorIndex = 0;

        spriteRenderer.color = playerColors[currColorIndex].GetSpriteColor();

        uiController.UpdateEffectText(playerColors[currColorIndex]); // update effect text
        uiController.UpdateClaimablesHUD(); // update claimables HUD (for selected indicator)

        // start cooldown
        canColorCycle = false;
        Invoke("ColorCycleCooldownComplete", colorCycleCooldown);

    }

    private void ColorCycleCooldownComplete() {

        canColorCycle = true;

    }

    public PlayerColor[] GetPlayerColors() { return playerColors; }

    public PlayerColor GetCurrentPlayerColor() { return playerColors[currColorIndex]; }

}
