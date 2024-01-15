using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClaimableInfo : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    [Header("Color")]
    private Color color;

    public void UpdateInfo(Color color, int amount) {

        this.color = color;
        icon.color = color;
        countText.text = amount + "";

    }

    public void UpdateInfo(int amount) {

        countText.text = amount + "";

    }

    public Color GetColor() { return color; }

}
