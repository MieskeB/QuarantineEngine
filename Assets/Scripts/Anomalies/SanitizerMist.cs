using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SanitizerMist : NetworkBehaviour, IAnomalyEffect
{
    [SerializeField] private float depletionRate = 1f;
    [SerializeField] private float maxTimeInMist = 5f;

    private readonly Dictionary<GameObject, float> exposureTimers = new();
    private readonly HashSet<GameObject> _affectedObjects = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || !other.CompareTag("Player")) return;
        _affectedObjects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer || !other.CompareTag("Player")) return;
        _affectedObjects.Remove(other.gameObject);
    }

    private void Update()
    {
        if (!IsServer) return;

        float deltaTime = Time.deltaTime;

        foreach (GameObject obj in _affectedObjects)
        {
            ApplyEffect(obj, deltaTime);
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (!_affectedObjects.Contains(obj))
            {
                RemoveEffect(obj);
            }
        }
    }

    public void ApplyEffect(GameObject target, float deltaTime)
    {
        if (!exposureTimers.ContainsKey(target))
            exposureTimers[target] = maxTimeInMist;

        exposureTimers[target] -= deltaTime;

        if (exposureTimers[target] <= 0f)
        {
            if (target.TryGetComponent(out PlayerNetworkController player))
            {
                player.DieServerRpc();
            }
        }
    }

    public void RemoveEffect(GameObject target)
    {
        if (exposureTimers.ContainsKey(target))
        {
            exposureTimers[target] = Mathf.Min(exposureTimers[target] + Time.deltaTime, maxTimeInMist);
        }
    }

    public float GetRemainingTime(GameObject target)
    {
        return exposureTimers.GetValueOrDefault(target, maxTimeInMist);
    }
}
