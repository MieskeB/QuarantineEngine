using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Rigidbody))]
public class PlayerNetworkController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump Settings")] [SerializeField]
    private float jumpForce = 6f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Crouch Settings")] [SerializeField]
    private float crouchHeight = 0.4f;

    [SerializeField] private float standHeight = 1f;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private float moveSpeedWhileCrouching = 2f;

    private Rigidbody _rb;
    private MouseLook _mouseLook;
    private Vector2 inputMovement;

    private bool isGrounded;
    private bool isJumping;
    private bool isCrouching;
    
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

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer,
            QueryTriggerInteraction.Ignore);
        if (isGrounded && _rb.velocity.y <= 0.1f)
        {
            isJumping = false;
        }
        
        // Movement
        Vector3 moveInput = new Vector3(inputMovement.x, 0f, inputMovement.y);
        Vector3 moveDirection = Camera.main.transform.TransformDirection(moveInput);
        moveDirection.y = 0f;
        moveDirection.Normalize();

        Vector3 velocity = moveDirection;
        if (isCrouching)
        {
            velocity *= moveSpeedWhileCrouching;
        }
        else
        {
            velocity *= moveSpeed;
        }

        Vector3 targetVelocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z); // keep gravity

        _rb.velocity = targetVelocity;
    }

    public void OnMove(Vector2 direction)
    {
        inputMovement = direction;
    }

    public void OnJump()
    {
        if (isGrounded && !isJumping)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
    }

    public void OnCrouch(bool enable)
    {
        if (enable && !isCrouching)
        {
            transform.localScale = new Vector3(1f, crouchHeight, 1f);
            isCrouching = true;
        }
        else if (!enable && isCrouching)
        {
            transform.localScale = new Vector3(1f, standHeight, 1f);
            isCrouching = false;
        }
    }

    [ServerRpc]
    public void DieServerRpc()
    {
        Die_Internal();
        DieClientRpc();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        // Trigger visual effects, disable input, show UI, etc.
    }

    private void Die_Internal()
    {
        Debug.Log("You deed");
        // Handle actual death logic: disable controller, mark as dead, respawn etc.
    }

}