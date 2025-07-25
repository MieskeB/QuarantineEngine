using System.Collections;
using TMPro;
using UnityEngine;

public class KeypadUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failureColor = Color.red;

    [SerializeField] private float feedbackDuration = 1f;
    
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
        
        controller.PlayClickSound();
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
            controller.PlaySuccessSound();
            StartCoroutine(HandleFeedback(successColor, true));
        }
        else
        {
            controller.PlayFailureSound();
            StartCoroutine(HandleFeedback(failureColor, false));
        }
    }
    
    private IEnumerator HandleFeedback(Color color, bool success)
    {
        if (displayText != null)
        {
            displayText.color = color;
        }

        yield return new WaitForSeconds(feedbackDuration);

        if (success)
        {
            OnClose(); // Correct code closes UI
        }
        else
        {
            OnClear();
            displayText.color = defaultColor;
        }
    }


    public void Unscrew()
    {
        if (controller.TryUnscrewPanel())
        {
            controller.PlayUnscrewSound();
            OnClose();
        }
        else
        {
            Debug.Log("No screwdriver in inventory");
        }
    }

    public void OnClose()
    {
        KeypadUIManager.Instance.CloseKeypad();
    }
}
