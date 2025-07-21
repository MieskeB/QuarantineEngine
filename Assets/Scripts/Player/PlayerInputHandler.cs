using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;
    private PlayerNetworkController _networkController;
    private PlayerInteract _playerInteract;
    private PlayerInventory _playerInventory;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _networkController = GetComponent<PlayerNetworkController>();
        _playerInteract = GetComponent<PlayerInteract>();
        _playerInventory = GetComponent<PlayerInventory>();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        
        _inputActions.Player.Move.performed += _networkController.OnMove;
        _inputActions.Player.Move.canceled += _networkController.OnMove;
        
        _inputActions.Player.Interact.performed += _playerInteract.TryInteract;

        _inputActions.Player.Slot1.performed += _ => _playerInventory.SelectSlot(0);
        _inputActions.Player.Slot2.performed += _ => _playerInventory.SelectSlot(1);
        _inputActions.Player.Slot3.performed += _ => _playerInventory.SelectSlot(2);
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= _networkController.OnMove;
        _inputActions.Player.Move.canceled -= _networkController.OnMove;
        
        _inputActions.Player.Interact.performed -= _playerInteract.TryInteract;
        
        _inputActions.Player.Slot1.performed -= _ => _playerInventory.SelectSlot(0);
        _inputActions.Player.Slot2.performed -= _ => _playerInventory.SelectSlot(1);
        _inputActions.Player.Slot3.performed -= _ => _playerInventory.SelectSlot(2);
        
        _inputActions.Player.Disable();
    }
}
