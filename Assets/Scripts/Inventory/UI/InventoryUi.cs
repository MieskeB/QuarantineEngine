using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InventoryUi : MonoBehaviour
{
    [Header("Optional Manual Binding")]
    [Tooltip("If left null, will attempt to bind to PlayerInventory.LocalInstance at runtime.")]
    [SerializeField] private PlayerInventory inventoryOverride;
    
    [Header("UI Setup")]
    [SerializeField] private RectTransform slotsRoot;
    [SerializeField] private InventoryUISlot slotPrefab;

    private PlayerInventory _inventory;
    private InventoryUISlot[] _slots;
    private bool _isBound;

    private void Awake()
    {
        if (slotsRoot == null)
        {
            slotsRoot = transform as RectTransform;
        }
    }

    private void OnEnable()
    {
        PlayerInventory.OnLocalInstanceChanged += HandleLocalInstanceChanged;
        TryBindToInventory();
    }

    private void OnDisable()
    {
        PlayerInventory.OnLocalInstanceChanged -= HandleLocalInstanceChanged;
        Unbind();
    }

    private void TryBindToInventory()
    {
        if (inventoryOverride != null)
        {
            Bind(inventoryOverride);
            return;
        }

        if (PlayerInventory.LocalInstance != null)
        {
            Bind(PlayerInventory.LocalInstance);
        }
    }

    private void HandleLocalInstanceChanged(PlayerInventory newInstance)
    {
        if (_inventory == newInstance)
        {
            return;
        }

        Unbind();

        if (newInstance != null)
        {
            Bind(newInstance);
        }
    }

    private void Bind(PlayerInventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogError("InventoryUI: Tried to bind to null PlayerInventory.");
            return;
        }

        _inventory = inventory;
        Subscribe();
        BuildSlots();
        RefreshAll();
        _isBound = true;
    }

    private void Unbind()
    {
        if (!_isBound)
        {
            return;
        }
        
        Unsubscribe();
        _inventory = null;
        _isBound = false;

        if (_slots != null)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null)
                {
                    _slots[i].SetItem(null);
                    _slots[i].SetSelected(false);
                }
            }
        }
    }
    
    private void Subscribe()
    {
        if (_inventory == null)
        {
            return;
        }

        _inventory.OnItemChanged += HandleItemChanged;
        _inventory.OnSelectedSlotChanged += HandleSelectedSlotChanged;
    }

    private void Unsubscribe()
    {
        if (_inventory == null)
        {
            return;
        }

        _inventory.OnItemChanged -= HandleItemChanged;
        _inventory.OnSelectedSlotChanged -= HandleSelectedSlotChanged;
    }

    private void BuildSlots()
    {
        if (slotPrefab == null)
        {
            Debug.LogError("InventoryUI: Slot prefab not assigned.");
            return;
        }

        for (int i = slotsRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = slotsRoot.GetChild(i);
            Destroy(child.gameObject);
        }

        int slotCount = _inventory.GetSlotCount();
        _slots = new InventoryUISlot[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            InventoryUISlot slotInstance = Instantiate(slotPrefab, slotsRoot);
            slotInstance.name = $"Slot_{i}";
            _slots[i] = slotInstance;
        }
    }

    private void HandleItemChanged(int slotIndex, InventoryItem item)
    {
        if (!_isBound)
        {
            return;
        }

        if (_slots == null || slotIndex < 0 || slotIndex >= _slots.Length)
        {
            return;
        }

        _slots[slotIndex].SetItem(item);
    }

    private void HandleSelectedSlotChanged(int slotIndex)
    {
        if (!_isBound)
        {
            return;
        }

        UpdateSelectedVisual(slotIndex);
    }

    private void RefreshAll()
    {
        if (_inventory == null || _slots == null)
        {
            return;
        }

        int count = _slots.Length;
        for (int i = 0; i < count; i++)
        {
            InventoryItem item = _inventory.GetItemInSlot(i);
            _slots[i].SetItem(item);
        }

        int selected = _inventory.GetSelectedSlot();
        UpdateSelectedVisual(selected);
    }

    private void UpdateSelectedVisual(int selectedSlotIndex)
    {
        if (_slots == null)
        {
            return;
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            bool isSelected = (i == selectedSlotIndex);
            _slots[i].SetSelected(isSelected);
        }
    }
}
