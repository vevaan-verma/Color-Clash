using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButton : CustomButton {

    [Header("Animations")]
    [SerializeField] protected Color hoverColor;
    [SerializeField] protected float hoverFadeDuration;
    [SerializeField][Tooltip("Check if other screens/HUDs fade over it")] private bool hasOverlays;
    protected Color startColor;

    private void Start() {

        startColor = text.color;

    }

    private void OnDisable() {

        // remove hover effects if this has overlays
        if (hasOverlays)
            text.DOColor(startColor, hoverFadeDuration).SetUpdate(true); // set update to true to ignore timescale

    }

    protected override void OnPointerEnter(PointerEventData eventData) {

        text.DOColor(hoverColor, hoverFadeDuration).SetUpdate(true); // color transition (set update to true to ignore timescale)

    }

    protected override void OnPointerExit(PointerEventData eventData) {

        text.DOColor(startColor, hoverFadeDuration).SetUpdate(true); // color transition (set update to true to ignore timescale)

    }
}
