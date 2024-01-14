using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButton : CustomButton {

    [Header("Animations")]
    [SerializeField] protected Color hoverColor;
    [SerializeField] protected float hoverFadeDuration;
    protected Color startColor;

    private void Start() {

        startColor = text.color;

    }

    private void OnDisable() {

        // remove hover effects
        text.DOColor(startColor, hoverFadeDuration);

    }

    protected override void OnPointerEnter(PointerEventData eventData) {

        text.DOColor(hoverColor, hoverFadeDuration); // color transition

    }

    protected override void OnPointerExit(PointerEventData eventData) {

        text.DOColor(startColor, hoverFadeDuration); // color transition

    }
}
