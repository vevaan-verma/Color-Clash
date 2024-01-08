using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyColorManager : MonoBehaviour {

    [Header("References")]
    private SpriteRenderer spriteRenderer;

    [Header("Color")]
    [SerializeField] private EnemyColor enemyColor;

    private void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();

        // color
        spriteRenderer.color = enemyColor.GetSpriteColor();

    }

    public EnemyColor GetCurrentEnemyColor() { return enemyColor; }

}
