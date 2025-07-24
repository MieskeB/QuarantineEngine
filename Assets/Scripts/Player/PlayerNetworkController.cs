using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Rigidbody))]
public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody _rb;
    private MouseLook _mouseLook;
    private Vector2 inputMovement;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mouseLook = GetComponent<MouseLook>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            _rb.isKinematic = true;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner || _mouseLook == null || _mouseLook.IsCameraFrozen)
        {
            return;
        }
        
        Vector3 moveInput = new Vector3(inputMovement.x, 0f, inputMovement.y);
        Vector3 moveDirection = Camera.main.transform.TransformDirection(moveInput);
        moveDirection.y = 0f;
        moveDirection.Normalize();

        Vector3 velocity = moveDirection * moveSpeed;
        Vector3 targetVelocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z); // keep gravity

        _rb.velocity = targetVelocity;
    }

    public void OnMove(Vector2 direction)
    {
        inputMovement = direction;
    }
}