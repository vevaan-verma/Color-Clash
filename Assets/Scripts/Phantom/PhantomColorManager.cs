using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhantomController))]
public class PhantomColorManager : MonoBehaviour {

    [Header("References")]
    private SpriteRenderer spriteRenderer;

    [Header("Color")]
    [SerializeField] private PhantomColor phantomColor;

    private void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();

        // color
        spriteRenderer.color = phantomColor.GetSpriteColor();

    }

    public PhantomColor GetCurrentEnemyColor() { return phantomColor; }

}
