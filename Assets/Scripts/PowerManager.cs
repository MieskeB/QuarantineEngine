using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerManager : NetworkBehaviour
{
    public static PowerManager Instance { get; private set; }

    [SerializeField] private float maxPower = 10f;

    private NetworkVariable<float> currentPowerUsage = new NetworkVariable<float>(0f);

    private readonly HashSet<IPoweredDevice> activeDevices = new HashSet<IPoweredDevice>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public bool CanUsePower(float amount)
    {
        return (currentPowerUsage.Value + amount) <= maxPower;
    }

    public void RegisterDevice(IPoweredDevice device)
    {
        if (IsServer && !activeDevices.Contains(device))
        {
            activeDevices.Add(device);
            currentPowerUsage.Value += device.PowerConsumption;
        }
    }

    public void UnregisterDevice(IPoweredDevice device)
    {
        if (IsServer && activeDevices.Contains(device))
        {
            activeDevices.Remove(device);
            currentPowerUsage.Value -= device.PowerConsumption;
        }
    }

    public float GetRemainingPower()
    {
        return maxPower - currentPowerUsage.Value;
    }

    public float GetCurrentUsage()
    {
        return currentPowerUsage.Value;
    }
}
