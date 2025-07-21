using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InventoryPickup : NetworkBehaviour, IInteractable
{
    [SerializeField] private InventoryItem itemData;
    
    public void Interact()
    {
        if (!IsServer)
        {
            InteractServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            TryPickup(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc(ulong clientId)
    {
        TryPickup(clientId);
    }

    private void TryPickup(ulong clientId)
    {
        NetworkManager.ConnectedClients.TryGetValue(clientId, out NetworkClient client);
        if (client != null && client.PlayerObject.TryGetComponent<PlayerInventory>(out PlayerInventory inventory))
        {
            inventory.AddItem(itemData);
            NetworkObject.Despawn();
        }
    }
}
