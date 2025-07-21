using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;
    private PlayerNetworkController _networkController;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _networkController = GetComponent<PlayerNetworkController>();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += _networkController.OnMove;
        _inputActions.Player.Move.canceled += _networkController.OnMove;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= _networkController.OnMove;
        _inputActions.Player.Move.canceled -= _networkController.OnMove;
        _inputActions.Player.Disable();
    }
}
