using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SecurityMonitor : NetworkBehaviour, IInteractable
{
    [SerializeField] private RawImage screen;
    private List<SecurityCamera> availableCameras;

    private NetworkVariable<int> currentCameraIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
        availableCameras = new List<SecurityCamera>(FindObjectsOfType<SecurityCamera>());
        currentCameraIndex.OnValueChanged += UpdateCameraFeed;
        UpdateCameraFeed(0, currentCameraIndex.Value);
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        currentCameraIndex.OnValueChanged -= UpdateCameraFeed;
    }
    
    private void UpdateCameraFeed(int previousIndex, int newIndex)
    {
        if (newIndex < 0 || newIndex >= availableCameras.Count)
            return;

        SecurityCamera selectedCamera = availableCameras[newIndex];
        screen.texture = selectedCamera.Render;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestCameraChangeServerRpc(ServerRpcParams rpcParams = default)
    {
        int nextIndex = currentCameraIndex.Value;
        int totalCameras = availableCameras.Count;

        for (int i = 0; i < totalCameras; i++)
        {
            nextIndex = (nextIndex + 1) % totalCameras;
            if (availableCameras[nextIndex].isOn)
            {
                currentCameraIndex.Value = nextIndex;
                break;
            }
        }
    }

    public void Interact()
    {
        if (availableCameras == null || availableCameras.Count == 0)
        {
            return;
        }
        
        if (IsServer)
        {
            int nextIndex = currentCameraIndex.Value;
            int totalCameras = availableCameras.Count;

            for (int i = 0; i < totalCameras; i++)
            {
                nextIndex = (nextIndex + 1) % totalCameras;
                if (availableCameras[nextIndex].isOn)
                {
                    currentCameraIndex.Value = nextIndex;
                    break;
                }
            }
        }
        else
        {
            RequestCameraChangeServerRpc();
        }
    }
}
