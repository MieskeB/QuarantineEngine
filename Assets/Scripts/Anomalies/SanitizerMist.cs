using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SanitizerMist : NetworkBehaviour, IAnomalyEffect
{
    [Header("Mist Settings")]
    [SerializeField] private float depletionRate = 1f;
    [SerializeField] private float maxTimeInMist = 5f;

    [Header("Roaming Settings")] 
    [SerializeField] private float roamRadius = 10f;
    [SerializeField] private float minRoamInterval = 3f;
    [SerializeField] private float maxRoamInterval = 6f;

    private readonly Dictionary<GameObject, float> exposureTimers = new();
    private readonly HashSet<GameObject> _affectedObjects = new();

    private NavMeshAgent agent;
    private float roamTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (!IsServer) return;

        ScheduleNextRoam();
    }
    
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

        // Apply effects
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

        // Roaming behavior
        roamTimer -= deltaTime;
        if (roamTimer <= 0f)
        {
            Vector3 randomDestination = GetRandomNavMeshLocation(transform.position, roamRadius);
            agent.SetDestination(randomDestination);
            ScheduleNextRoam();
        }
    }

    private void ScheduleNextRoam()
    {
        roamTimer = Random.Range(minRoamInterval, maxRoamInterval);
    }

    private Vector3 GetRandomNavMeshLocation(Vector3 origin, float radius)
    {
        for (int i = 0; i < 10; i++) // Try max 10 times to find a point
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return origin; // fallback to current position
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
