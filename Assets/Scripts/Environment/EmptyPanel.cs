using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyPanel : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorController linkedDoor;
    private bool isBypassed = false;
    
    public void Interact()
    {
        // TODO only if player has screwdriver in inventory
        Debug.LogWarning("TODO");
    }
}
