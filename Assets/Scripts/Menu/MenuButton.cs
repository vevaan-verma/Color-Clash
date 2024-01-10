using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour {

    [Header("References")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private Slider underline;

    [Header("Animations")]
    [SerializeField] private Color hoverColor;
    [SerializeField] private float hoverFadeDuration;
    [SerializeField] private float underlineDuration;
    private Color startColor;


    private void Start() {

        EventTrigger eventTriggers = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener((data) => OnPointerEnter((PointerEventData) data));

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener((data) => OnPointerExit((PointerEventData) data));

        eventTriggers.triggers.Add(entry1);
        eventTriggers.triggers.Add(entry2);

        startColor = text.color;

        underline.value = 0f;

    }

    private void OnPointerEnter(PointerEventData eventData) {

        text.DOColor(hoverColor, hoverFadeDuration); // color transition
        underline.DOValue(underline.maxValue, underlineDuration).SetEase(Ease.InFlash); // underline

    }

    private void OnPointerExit(PointerEventData eventData) {

        text.DOColor(startColor, hoverFadeDuration); // color transition
        underline.DOValue(0f, underlineDuration).SetEase(Ease.OutFlash); // underline

    }
}
