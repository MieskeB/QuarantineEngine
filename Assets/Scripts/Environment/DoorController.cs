using Unity.Netcode;
using UnityEngine;

public class DoorController : NetworkBehaviour
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private float openSpeed = 2f;

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

    [ServerRpc(RequireOwnership = false)]
    private void ToggleDoorServerRpc()
    {
        isOpen.Value = !isOpen.Value;
    }
}