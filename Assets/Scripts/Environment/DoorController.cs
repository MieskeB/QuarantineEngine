using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DoorController : NetworkBehaviour, IPoweredDevice
{
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float powerConsumption = 2f;

    [SerializeField] private string code = "1234";
    [SerializeField] private float doorTimer = 7f;

    private NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);
    
    private Coroutine autoCloseCoroutine;

    public float PowerConsumption => powerConsumption;

    private void Update()
    {
        Vector3 target = isOpen.Value ? openPosition : closedPosition;
        doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, target, Time.deltaTime * openSpeed);
    }

    public void ToggleDoor()
    {
        if (!IsServer)
        {
            ToggleDoorServerRpc();
            return;
        }

        if (!isOpen.Value)
        {
            if (!PowerManager.Instance.CanUsePower(powerConsumption))
            {
                return;
            }

            PowerManager.Instance.RegisterDevice(this);
        }
        else
        {
            PowerManager.Instance.UnregisterDevice(this);
        }
        
        isOpen.Value = !isOpen.Value;
    }

    public void Open()
    {
        if (!IsServer)
        {
            OpenDoorServerRpc();
            return;
        }

        if (!isOpen.Value && PowerManager.Instance.CanUsePower(powerConsumption))
        {
            PowerManager.Instance.RegisterDevice(this);
            isOpen.Value = true;
        }
    }

    public bool TryUnlock(string code)
    {
        if (this.code != code)
        {
            return false;
        }
        
        bool wasToggledOpen = isOpen.Value == false;

        ToggleDoor();

        if (IsServer && wasToggledOpen && isOpen.Value == true)
        {
            StartAutoCloseTimer();
        }
        else if (!IsServer && wasToggledOpen && isOpen.Value == true)
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

        if (isOpen.Value)
        {
            isOpen.Value = false;
            PowerManager.Instance.UnregisterDevice(this);
        }
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
            if (autoCloseCoroutine != null)
            {
                StopCoroutine(autoCloseCoroutine);
                autoCloseCoroutine = null;
            }
        }
        else
        {
            StopCloseTimerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopCloseTimerServerRpc()
    {
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
        }
    }
}