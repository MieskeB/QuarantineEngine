using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;
    private PlayerNetworkController _networkController;
    private PlayerInteract _playerInteract;
    private PlayerInventory _playerInventory;
    private PlayerInventoryDropper _dropper;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _networkController = GetComponent<PlayerNetworkController>();
        _playerInteract = GetComponent<PlayerInteract>();
        _playerInventory = GetComponent<PlayerInventory>();
        _dropper = GetComponent<PlayerInventoryDropper>();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        
        _inputActions.Player.Interact.performed += TryInteract;

        _inputActions.Player.Slot1.performed += SelectSlot1;
        _inputActions.Player.Slot2.performed += SelectSlot2;
        _inputActions.Player.Slot3.performed += SelectSlot3;

        _inputActions.Player.Drop.performed += OnDrop;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;
        
        _inputActions.Player.Interact.performed -= TryInteract;
        
        _inputActions.Player.Slot1.performed -= SelectSlot1;
        _inputActions.Player.Slot2.performed -= SelectSlot2;
        _inputActions.Player.Slot3.performed -= SelectSlot3;
        
        _inputActions.Player.Drop.performed -= OnDrop;
        
        _inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _networkController.OnMove(context.ReadValue<Vector2>());
    }

    private void TryInteract(InputAction.CallbackContext context)
    {
        _playerInteract.TryInteract();
    }

    private void SelectSlot1(InputAction.CallbackContext context)
    {
        _playerInventory.SelectSlot(0);
    }
    
    private void SelectSlot2(InputAction.CallbackContext context)
    {
        _playerInventory.SelectSlot(1);
    }
    
    private void SelectSlot3(InputAction.CallbackContext context)
    {
        _playerInventory.SelectSlot(2);
    }

    private void OnDrop(InputAction.CallbackContext context)
    {
        _dropper.RequestDropSelected();
    }
}
