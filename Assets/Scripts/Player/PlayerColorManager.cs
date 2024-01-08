using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerColorManager : MonoBehaviour {

    [Header("Color Cycling")]
    [SerializeField] private PlayerColor[] playerColors;
    [SerializeField] private float colorCycleCooldown;
    private int currColorIndex;
    private SpriteRenderer spriteRenderer;
    private bool canColorCycle;

    [Header("Keybinds")]
    [SerializeField] private KeyCode colorSwitchKey;

    private void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();

        currColorIndex = 0;
        spriteRenderer.color = playerColors[currColorIndex].GetSpriteColor();

        canColorCycle = true;

    }

    private void Update() {

        // color switching
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
