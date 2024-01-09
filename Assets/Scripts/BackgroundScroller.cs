using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {

    [Header("Scrolling")]
    [SerializeField][Range(-1f, 1f)] private float xScrollSpeed;
    [SerializeField][Range(-1f, 1f)] private float yScrollSpeed;
    private float xOffset;
    private float yOffset;
    private Material material;

    private void Start() {

        material = GetComponent<Renderer>().material;

    }

    private void Update() {

        xOffset += Time.deltaTime * xScrollSpeed / 10f; // divide by 10 to reduce scrolling speed
        yOffset += Time.deltaTime * yScrollSpeed / 10f; // divide by 10 to reduce scrolling speed
        material.SetTextureOffset("_MainTex", new Vector2(xOffset, xOffset));

    }
}
