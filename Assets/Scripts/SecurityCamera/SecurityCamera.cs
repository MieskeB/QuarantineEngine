using System;
using Unity.Netcode;
using UnityEngine;

public class SecurityCamera : NetworkBehaviour, IInteractable, IPoweredDevice
{
    [Header("Camera settings")]
    [SerializeField] private Camera cam;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private float powerConsumption = 2f;

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
    public float PowerConsumption => powerConsumption;

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
        SetPowered(!isCameraOn.Value);
    }

    private void SetPowered(bool on)
    {
        if (on)
        {
            if (IsServer && PowerManager.Instance.CanUsePower(powerConsumption))
            {
                PowerManager.Instance.RegisterDevice(this);
                isCameraOn.Value = true;
            }
        }
        else
        {
            PowerManager.Instance.UnregisterDevice(this);
            isCameraOn.Value = false;
        }
    }
}