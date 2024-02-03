using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplierInfo : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text multiplierText;
    [SerializeField] private float transitionDuration;
    private float currMultiplier;

    public void UpdateInfo(Sprite icon, float multiplier) {

        this.icon.sprite = icon;
        DOVirtual.Float(currMultiplier, multiplier, transitionDuration, (value) => { multiplierText.text = (Mathf.Round(value * 100f) / 100f) + "x"; }).OnComplete(() => currMultiplier = multiplier); // round multiplier to 2 decimal places
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>()); // force rebuild layout to update multiplier text width

    }
}
