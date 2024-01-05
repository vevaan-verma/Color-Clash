using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;

public class Claimable : MonoBehaviour {

    [Header("References")]
    private SpriteRenderer spriteRenderer;

    [Header("Animations")]
    [SerializeField] private float colorFadeDuration;
    private Coroutine colorCoroutine;

    private void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    public void FadeColor(Color targetColor) {

        if (colorCoroutine != null)
            StopCoroutine(colorCoroutine);

        colorCoroutine = StartCoroutine(AnimateColor(targetColor, colorFadeDuration));

    }

    private IEnumerator AnimateColor(Color targetColor, float duration) {

        float currentTime = 0f;
        Color startColor = spriteRenderer.color;

        while (currentTime < duration) {

            currentTime += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, currentTime / duration);
            yield return null;

        }

        spriteRenderer.color = targetColor;
        colorCoroutine = null;

    }
}
