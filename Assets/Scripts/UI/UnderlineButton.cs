using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnderlineButton : CustomButton {

    [Header("References")]
    [SerializeField] private Slider underline;

    [Header("Animations")]
    [SerializeField] private float underlineDuration;

    private void Start() {

        underline.value = 0f;

    }

    private void OnDisable() {

        // remove underlines
        underline.DOValue(0f, underlineDuration).SetEase(Ease.OutFlash).SetUpdate(true); // set update to true to ignore timescale

    }

    protected override void OnPointerEnter(PointerEventData eventData) {

        underline.DOValue(underline.maxValue, underlineDuration).SetEase(Ease.InFlash).SetUpdate(true); // underline  (set update to true to ignore timescale)

    }

    protected override void OnPointerExit(PointerEventData eventData) {

        underline.DOValue(0f, underlineDuration).SetEase(Ease.OutFlash).SetUpdate(true); // underline (set update to true to ignore timescale)

    }
}
