using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerNetworkController : NetworkBehaviour
{
    private CharacterController _characterController;
    private MouseLook _mouseLook;
    private Vector2 inputMovement;

    [SerializeField] private float moveSpeed = 5f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _mouseLook = GetComponent<MouseLook>();
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
        if (!IsOwner || !Camera.main) return;

        if (_mouseLook.IsCameraFrozen)
        {
            return;
        }
        
        Vector3 move = new Vector3(inputMovement.x, 0f, inputMovement.y);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0f;

        _characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    public void OnMove(Vector2 direction)
    {
        inputMovement = direction;
    }
}