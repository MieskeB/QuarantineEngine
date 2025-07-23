using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] private int slotCount = 3;

    private List<InventoryItem> items = new List<InventoryItem>();
    private NetworkVariable<int> selectedSlot = new NetworkVariable<int>(0);

    public static PlayerInventory LocalInstance { get; private set; }

    public event Action<int, InventoryItem> OnItemChanged;
    public event Action<int> OnSelectedSlotChanged;

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

        selectedSlot.OnValueChanged += HandleSelectedSlotChanged;
    }

    public override void OnDestroy()
    {
        selectedSlot.OnValueChanged -= HandleSelectedSlotChanged;
        if (LocalInstance == this)
        {
            LocalInstance = null;
        }
    }

    private void HandleSelectedSlotChanged(int previousValue, int newValue)
    {
        if (OnSelectedSlotChanged != null)
        {
            OnSelectedSlotChanged.Invoke(newValue);
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

    public bool AddItem(InventoryItem item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                if (OnItemChanged != null)
                {
                    OnItemChanged.Invoke(i, item);
                }

                return true;
            }
        }

        Debug.Log("Inventory full");
        return false;
    }

    public InventoryItem GetSelectedItem()
    {
        return items[selectedSlot.Value];
    }

    public int GetSelectedSlot()
    {
        return selectedSlot.Value;
    }

    public int GetSlotCount()
    {
        return items.Count;
    }

    public InventoryItem GetItemInSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count)
        {
            return null;
        }

        return items[slotIndex];
    }

    public bool ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count)
        {
            return false;
        }

        if (items[slotIndex] == null)
        {
            return false;
        }

        items[slotIndex] = null;
        if (OnItemChanged != null)
        {
            OnItemChanged.Invoke(slotIndex, null);
        }

        return true;
    }
}