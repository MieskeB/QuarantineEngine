using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SecurityMonitor : NetworkBehaviour, IInteractable
{
    [SerializeField] private RawImage screen;
    [SerializeField] private List<SecurityCamera> availableCameras;

    private NetworkVariable<int> currentCameraIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
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
        int nextIndex = (currentCameraIndex.Value + 1) % availableCameras.Count;
        currentCameraIndex.Value = nextIndex;
    }

    public void Interact()
    {
        if (IsServer)
        {
            int nextIndex = (currentCameraIndex.Value + 1) % availableCameras.Count;
            currentCameraIndex.Value = nextIndex;
        }
        else
        {
            RequestCameraChangeServerRpc();
        }
    }
}
