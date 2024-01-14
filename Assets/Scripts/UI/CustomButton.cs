using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CustomButton : MonoBehaviour {

    [Header("References")]
    [SerializeField] protected TMP_Text text;

    public void Awake() { // runs in all classes, don't override, use start for unique method

        EventTrigger eventTriggers = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener((data) => OnPointerEnter((PointerEventData) data));

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener((data) => OnPointerExit((PointerEventData) data));

        eventTriggers.triggers.Add(entry1);
        eventTriggers.triggers.Add(entry2);

    }

    protected abstract void OnPointerEnter(PointerEventData eventData);

    protected abstract void OnPointerExit(PointerEventData eventData);

}
