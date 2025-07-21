using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerNetworkController : NetworkBehaviour
{
    private CharacterController _characterController;
    private Vector2 inputMovement;

    [SerializeField] private float moveSpeed = 5f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (!Camera.main) return;
        
        Vector3 move = new Vector3(inputMovement.x, 0f, inputMovement.y);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0f;

        _characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMovement = context.ReadValue<Vector2>();
    }
}
