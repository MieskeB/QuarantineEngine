using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : MonoBehaviour, IInteractable
{
    [SerializeField] private string correctCode = "1234";
    [SerializeField] private DoorController linkedDoor;
    [SerializeField] private GameObject keypadUI;

    private string currentInput = "";

    public void Interact()
    {
        keypadUI.SetActive(true);
        // TODO enable cursor
    }

    public void AddDigit(string digit)
    {
        currentInput += digit;
    }

    public void SubmitCode()
    {
        if (currentInput == correctCode)
        {
            linkedDoor.ToggleDoor();
            keypadUI.SetActive(false);
        }
        else
        {
            Debug.Log("Incorrect code");
        }
        currentInput = "";
    }
}
