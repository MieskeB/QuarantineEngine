using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeypadUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    private string currentInput;
    private KeypadController controller;

    public void Init(KeypadController keypad)
    {
        controller = keypad;
    }

    public void OnNumberPressed(string number)
    {
        currentInput += number;
        displayText.text = currentInput;
    }

    public void OnClear()
    {
        currentInput = "";
        displayText.text = currentInput;
    }

    public void OnSubmit()
    {
        if (controller.SubmitCode(currentInput))
        {
            KeypadUIManager.Instance.CloseKeypad();
        }
        else
        {
            OnClear();
        }
    }
}
