using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInventoryDropper : NetworkBehaviour
{
    [Header("Drop Settings")] [SerializeField]
    private Transform dropOrigin;

    [SerializeField] private float forwardSpawnOffset = 0.5f;
    [SerializeField] private float forwardThrowForce = 2.0f;

    private PlayerInventory _inventory;

    private void Awake()
    {
        _inventory = GetComponent<PlayerInventory>();
        if (dropOrigin == null)
        {
            dropOrigin = transform;
        }
    }

    public void RequestDropSelected()
    {
        if (!IsOwner)
        {
            return;
        }

        DropSelectedServerRpc();
    }

    [ServerRpc]
    private void DropSelectedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (_inventory == null)
        {
            return;
        }

        int slotIndex = _inventory.GetSelectedSlot();
        InventoryItem item = _inventory.GetItemInSlot(slotIndex);
        Debug.Log(item.itemName);
        Debug.Log(item.itemPrefab); // <-- this is null
        Debug.Log(item.itemIcon);

        if (item == null || item.itemPrefab == null)
        {
            return;
        }

        InventoryItem removedItem = _inventory.RemoveItemAtSlot(slotIndex);
        if (removedItem == null)
        {
            return;
        }

        Vector3 spawnPos = dropOrigin.position + dropOrigin.forward * forwardSpawnOffset;
        Quaternion spawnRot = Quaternion.identity;

        GameObject go = Instantiate(removedItem.itemPrefab, spawnPos, spawnRot);
        
        NetworkObject netObj = go.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }

        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 impulse = dropOrigin.forward * forwardThrowForce;
            rb.AddForce(impulse, ForceMode.VelocityChange);
        }
    }
}
