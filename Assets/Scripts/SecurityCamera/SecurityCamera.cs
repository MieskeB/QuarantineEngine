using System;
using Unity.Netcode;
using UnityEngine;

public class SecurityCamera : NetworkBehaviour, IInteractable
{
    [Header("Camera settings")]
    [SerializeField] private Camera cam;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private float powerConsumption = 1f;

    [Header("Rotation settings")] [SerializeField]
    private Transform rotationPivot;

    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float leftLimit = -45f;
    [SerializeField] private float rightLimit = 45f;
    [SerializeField] private bool rotateAutomatically = true;

    private float currentAngle;
    private int direction = 1;
    
    private NetworkVariable<bool> isCameraOn = new NetworkVariable<bool>(
        value: false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public Camera UnityCamera => cam;
    public RenderTexture Render => renderTexture;

    private void Start()
    {
        isCameraOn.OnValueChanged += HandleCameraStateChanged;
        ToggleCamera(isCameraOn.Value);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        isCameraOn.OnValueChanged -= HandleCameraStateChanged;
    }

    private void HandleCameraStateChanged(bool previous, bool current)
    {
        ToggleCamera(current);
    }

    private void ToggleCamera(bool isOn)
    {
        if (cam == null)
        {
            return;
        }
        
        cam.enabled = isOn;
        cam.targetTexture = isOn ? renderTexture : null;
    }

    private void Update()
    {
        if (!rotateAutomatically || rotationPivot == null || !isCameraOn.Value)
        {
            return;
        }

        float deltaAngle = rotationSpeed * Time.deltaTime * direction;
        currentAngle += deltaAngle;
        rotationPivot.Rotate(Vector3.up, deltaAngle, Space.Self);

        if (currentAngle >= rightLimit || currentAngle <= leftLimit)
        {
            direction *= -1;
        }
    }

    public void SetAutoRotate(bool enable)
    {
        rotateAutomatically = enable;
    }

    public void Interact()
    {
        if (IsServer)
        {
            ToggleCamera();
        }
        else
        {
            RequestToggleCameraServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestToggleCameraServerRpc(ServerRpcParams rpcParams = default)
    {
        ToggleCamera();
    }

    private void ToggleCamera()
    {
        isCameraOn.Value = !isCameraOn.Value;
    }
}