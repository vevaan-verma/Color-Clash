using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {

    [Header("Scrolling")]
    [SerializeField][Range(-1f, 1f)] private float scrollSpeed;
    private float offset;
    private Material material;

    private void Start() {

        material = GetComponent<Renderer>().material;

    }

    private void Update() {

        offset += Time.deltaTime * scrollSpeed / 10f; // divide by 10 to reduce scrolling speed
        material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));

    }
}
