
using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectionHighlight;

    public void SetItem(InventoryItem item)
    {
        if (item != null && item.itemIcon != null)
        {
            iconImage.sprite = item.itemIcon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(isSelected);
        }

        transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
    }
}
