using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KeypadController : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform cameraFocusPoint;
    // [SerializeField] private GameObject keypadUIPrefab;
    [SerializeField] private bool isPanelUnscrewed = false;
    [SerializeField] private DoorController connectedDoor;

    public void Interact()
    {
        if (!NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)
        {
            return;
        }
        
        MouseLook look = FindObjectOfType<MouseLook>();
        look.EnterFocusMode(cameraFocusPoint);
        
        KeypadUIManager.Instance.OpenKeypad(this);
    }

    public void UnscrewPanel()
    {
        isPanelUnscrewed = true;
    }

    public void TryHotWire(bool hasDuctTape)
    {
        if (!isPanelUnscrewed)
        {
            return;
        }

        if (hasDuctTape)
        {
            connectedDoor.Open();
        }
    }

    public void TryInstallRaspberryPi(ProgrammableDevice device)
    {
        if (!isPanelUnscrewed)
        {
            return;
        }

        // connectedDoor.AssignAutomation(device);
    }

    public bool SubmitCode(string code)
    {
        if (connectedDoor != null)
        {
            return connectedDoor.TryUnlock(code);
        }

        Debug.LogError("No door is connected to this device");
        return false;
    }
}
