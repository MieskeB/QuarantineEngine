using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioController : NetworkBehaviour
{
    [Header("Audio Clips")] [SerializeField]
    private AudioClip coughClip;

    [Header("Cough Settings")] [SerializeField]
    private float minCoughInterval = 1f;

    [SerializeField] private float maxCoughInterval = 2f;

    private AudioSource audioSource;
    private float nextCoughTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    public void TryCough()
    {
        if (Time.time >= nextCoughTime)
        {
            nextCoughTime = Time.time + Random.Range(minCoughInterval, maxCoughInterval);
            PlayCoughClientRpc();
        }
    }

    [ClientRpc]
    private void PlayCoughClientRpc()
    {
        if (coughClip != null)
        {
            audioSource.PlayOneShot(coughClip);
        }
    }
}
