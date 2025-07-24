using Unity.Netcode;
using UnityEngine;

public class SecurityCamera : NetworkBehaviour
{
    [Header("Camera settings")]
    [SerializeField] private Camera cam;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private float powerConsumption = 1f;

    [Header("Rotation settings")] [SerializeField]
    private Transform rotationPivot;

    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float leftLimit = -45f;
    [SerializeField] private float rightLimit = 45f;
    [SerializeField] private bool rotateAutomatically = true;

    private float currentAngle;
    private int direction = 1;

    public Camera UnityCamera => cam;
    public RenderTexture Render => renderTexture;

    public void ToggleCamera(bool isOn)
    {
        cam.enabled = isOn;
        cam.targetTexture = isOn ? renderTexture : null;
    }

    private void Update()
    {
        if (!rotateAutomatically || rotationPivot == null)
        {
            return;
        }

        float deltaAngle = rotationSpeed * Time.deltaTime * direction;
        currentAngle += deltaAngle;
        rotationPivot.Rotate(Vector3.up, deltaAngle, Space.Self);

        if (currentAngle >= rightLimit || currentAngle <= leftLimit)
        {
            direction *= -1;
        }
    }

    public void SetAutoRotate(bool enable)
    {
        rotateAutomatically = enable;
    }
}