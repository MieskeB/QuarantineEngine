using Unity.Netcode;
using UnityEngine;

public class DoorController : NetworkBehaviour
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private float openSpeed = 2f;

    [SerializeField] private string code = "1234";

    private NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);

    private void Update()
    {
        Vector3 target = isOpen.Value ? openPosition : closedPosition;
        doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, target, Time.deltaTime * openSpeed);
    }

    public void ToggleDoor()
    {
        if (IsServer)
        {
            isOpen.Value = !isOpen.Value;
        }
        else
        {
            ToggleDoorServerRpc();
        }
    }

    public void Open()
    {
        if (IsServer)
        {
            isOpen.Value = true;
        }
        else
        {
            ToggleDoorServerRpc();
        }
    }

    public bool TryUnlock(string code)
    {
        if (this.code != code)
        {
            return false;
        }

        ToggleDoor();
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleDoorServerRpc()
    {
        isOpen.Value = !isOpen.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    private void OpenDoorServerRpc()
    {
        isOpen.Value = true;
    }
    
    
}