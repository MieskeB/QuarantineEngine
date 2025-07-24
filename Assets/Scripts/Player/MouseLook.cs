using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float cameraLerpSpeed = 10f;

    private float xRotation = 0f;

    private bool isInFocusMode = false;
    private bool isReturningFromFocus = false;

    private Vector3 focusTargetPosition;
    private Quaternion focusTargetRotation;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private void Start()
    {
        if (!IsOwner)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;

        originalCameraPosition = cameraHolder.position;
        originalCameraRotation = cameraHolder.rotation;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (isInFocusMode)
        {
            MoveCameraToFocus(focusTargetPosition, focusTargetRotation);
        }
        else if (isReturningFromFocus)
        {
            MoveCameraToFocus(originalCameraPosition, originalCameraRotation);

            if (Vector3.Distance(cameraHolder.position, originalCameraPosition) < 0.01f &&
                Quaternion.Angle(cameraHolder.rotation, originalCameraRotation) < 0.5f)
            {
                isReturningFromFocus = false;
            }
        }
        else
        {
            RotateWithMouse();
        }
    }

    private void RotateWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void MoveCameraToFocus(Vector3 targetPos, Quaternion targetRot)
    {
        cameraHolder.position = Vector3.Lerp(cameraHolder.position, targetPos, Time.deltaTime * cameraLerpSpeed);
        cameraHolder.rotation = Quaternion.Lerp(cameraHolder.rotation, targetRot, Time.deltaTime * cameraLerpSpeed);
    }

    public void EnterFocusMode(Transform focusPoint, float focusDistance = 0.5f)
    {
        isInFocusMode = true;
        
        Vector3 targetPosition = focusPoint.position - focusPoint.forward * focusDistance;
        Quaternion targetRotation = Quaternion.LookRotation(focusPoint.forward);

        focusTargetPosition = targetPosition;
        focusTargetRotation = targetRotation;

        originalCameraPosition = cameraHolder.position;
        originalCameraRotation = cameraHolder.rotation;

        Cursor.lockState = CursorLockMode.None;
    }

    public void ExitFocusMode()
    {
        isInFocusMode = false;
        isReturningFromFocus = true;
        focusTargetPosition = Vector3.zero;
        focusTargetRotation = Quaternion.identity;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public bool IsCameraFrozen => isInFocusMode || isReturningFromFocus;
}