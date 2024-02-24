using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClaimableInfo : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private GameObject selectedIndicator;

    [Header("Color")]
    private Color color;

    public void UpdateInfo(Color color, int amount, bool selected) {

        this.color = color;
        icon.color = color;
        countText.text = amount + "";
        selectedIndicator.SetActive(selected); // update selected indicator

    }

    public void UpdateInfo(int amount, bool selected) {

        countText.text = amount + "";
        selectedIndicator.SetActive(selected); // update selected indicator

    }

    public Color GetColor() { return color; }

}
