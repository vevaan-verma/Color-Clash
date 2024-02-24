using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {

    [Header("Scrolling")]
    [SerializeField] private float xScrollSpeed;
    [SerializeField] private float yScrollSpeed;
    private float xOffset;
    private float yOffset;
    private Material material;

    private void Start() {

        material = GetComponent<Renderer>().material;

    }

    private void Update() {

        xOffset += Time.deltaTime * xScrollSpeed / 10f; // divide by 10 to reduce scrolling speed
        yOffset += Time.deltaTime * yScrollSpeed / 10f; // divide by 10 to reduce scrolling speed
        material.SetTextureOffset("_MainTex", new Vector2(xOffset, yOffset));

    }
}
