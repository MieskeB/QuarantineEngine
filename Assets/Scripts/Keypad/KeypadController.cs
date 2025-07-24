using Unity.Netcode;
using UnityEngine;

public class KeypadController : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private bool isPanelUnscrewed = false;
    [SerializeField] private DoorController connectedDoor;

    private bool isHotWired = false;
    private bool isRaspberryPi = false;

    public void Interact()
    {
        if (!NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)
        {
            return;
        }

        if (!isPanelUnscrewed)
        {
            MouseLook look = FindObjectOfType<MouseLook>();
            look.EnterFocusMode(cameraFocusPoint);

            KeypadUIManager.Instance.OpenKeypad(this);
        }
        else
        {
            PlayerInventory inventory = PlayerInventory.LocalInstance;
            InventoryItem item = inventory.GetSelectedItem();
            if (item == null)
            {
                return;
            }

            if (item.itemName == "Tape")
            {
                TryHotWire();
            }
            else if (item.itemName == "Raspberry Pi")
            {
                Debug.Log("Installed Raspberry Pi");
                // TryInstallRaspberryPi();
            }
            else
            {
                return;
            }

            inventory.RemoveSelectedItem();
        }
    }

    public bool TryUnscrewPanel()
    {
        if (isPanelUnscrewed)
        {
            Debug.LogWarning("Panel is already unscrewed.");
            return false;
        }

        if (PlayerInventory.LocalInstance.ContainsItem("Screwdriver"))
        {
            isPanelUnscrewed = true;
            return true;
        }

        return false;
    }

    private void TryHotWire()
    {
        if (!isPanelUnscrewed || isRaspberryPi)
        {
            return;
        }

        connectedDoor.StopCloseTimer();
        connectedDoor.Open();
        isHotWired = true;
    }

    public void TryInstallRaspberryPi(ProgrammableDevice device)
    {
        if (!isPanelUnscrewed)
        {
            return;
        }

        // connectedDoor.AssignAutomation(device);
        isRaspberryPi = true;
    }

    public bool SubmitCode(string code)
    {
        if (isPanelUnscrewed)
        {
            return false;
        }

        if (connectedDoor != null)
        {
            return connectedDoor.TryUnlock(code);
        }

        Debug.LogError("No door is connected to this device");
        return false;
    }
}