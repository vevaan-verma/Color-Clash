using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodeManager : MonoBehaviour {

    [Header("Code Digits")]
    [SerializeField] private TMP_Text[] codeDigitTexts;
    private int[] codeDigits;

    private void Start() {

        codeDigits = new int[codeDigitTexts.Length];

        // randomize code digits each time
        for (int i = 0; i < codeDigitTexts.Length; i++)
            codeDigitTexts[i].text = (codeDigits[i] = Random.Range(0, 10)) + "";

    }

    public bool CheckCode(string inputCode) {

        if (inputCode.Length != codeDigits.Length) return false; // codes aren't same length

        // check if input code is equal to actual code
        for (int i = 0; i < inputCode.Length; i++)
            if (inputCode[i] - '0' != codeDigits[i]) // subtract '0' to convert to int
                return false;

        return true;

    }
}
