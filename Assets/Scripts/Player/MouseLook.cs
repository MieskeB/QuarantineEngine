using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 100f;

    private float xRotation = 0f;

    private void Start()
    {
        if (!IsOwner)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            enabled = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!IsOwner) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}