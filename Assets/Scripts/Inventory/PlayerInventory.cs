using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] private int slotCount = 5;

    private List<InventoryItem> items = new List<InventoryItem>();
    private NetworkVariable<int> selectedSlot = new NetworkVariable<int>();
    
    public static PlayerInventory LocalInstance { get; private set; }

    private void Awake()
    {
        for (int i = 0; i < slotCount; i++)
        {
            items.Add(null);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            SelectSlotServerRpc(slotIndex);
        }
    }

    [ServerRpc]
    private void SelectSlotServerRpc(int slot)
    {
        selectedSlot.Value = slot;
    }

    public void AddItem(InventoryItem item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                return;
            }
        }
        
        Debug.Log("Inventory full");
    }

    public InventoryItem GetSelectedItem()
    {
        return items[selectedSlot.Value];
    }

    public int GetSelectedSlot() => selectedSlot.Value;
}
