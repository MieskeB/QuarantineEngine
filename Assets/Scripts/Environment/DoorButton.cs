using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorController targetDoor;
    
    public void Interact()
    {
        targetDoor.ToggleDoor();
    }
}
