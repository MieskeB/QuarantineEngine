using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DoorController : NetworkBehaviour
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private float openSpeed = 2f;

    [SerializeField] private string code = "1234";
    [SerializeField] private float doorTimer = 7f;

    private NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);
    
    private Coroutine autoCloseCoroutine;

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
            OpenDoorServerRpc();
        }
    }

    public bool TryUnlock(string code)
    {
        if (this.code != code)
        {
            return false;
        }

        ToggleDoor();

        if (IsServer)
        {
            StartAutoCloseTimer();
        }
        else
        {
            StartAutoCloseServerRpc();
        }
        
        return true;
    }

    private void StartAutoCloseTimer()
    {
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }

        autoCloseCoroutine = StartCoroutine(AutoCloseAfterDelay());
    }

    private IEnumerator AutoCloseAfterDelay()
    {
        yield return new WaitForSeconds(doorTimer);
        isOpen.Value = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartAutoCloseServerRpc()
    {
        StartAutoCloseTimer();
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

    public void StopCloseTimer()
    {
        if (IsServer)
        {
            StopCoroutine(autoCloseCoroutine);
        }
        else
        {
            StopCloseTimerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopCloseTimerServerRpc()
    {
        StopCoroutine(autoCloseCoroutine);
    }
}