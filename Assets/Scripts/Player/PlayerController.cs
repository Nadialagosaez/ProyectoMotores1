using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerMovement motorMovimiento;

    [Header("Humanoid Rotation")]
    [SerializeField] private float sensitivity = 0.3f;
    [SerializeField] private float maxLookAngle = 80f;
    [Range(0.01f, 0.5f)] [SerializeField] private float smoothTime = 0.05f;

    [Header("Walk (Cinemachine Bob)")]
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float idleAmplitude = 0.2f;
    [SerializeField] private float walkAmplitude = 2.8f;
    [SerializeField] private float walkFrequency = 2.2f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private Vector2 currentLookDelta;
    private Vector2 lookSmoothVelocity;
    private CinemachineBasicMultiChannelPerlin noiseComponent;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (motorMovimiento == null)
            motorMovimiento = GetComponent<PlayerMovement>();

        if (virtualCamera != null)
        {
            noiseComponent = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnRotate(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
    {
        if (motorMovimiento == null) return;

        if (context.performed)
        {
            motorMovimiento.Jump();
        }
    }
    
    void Update()
    {
        HandleRotation();
        SendMovementToMotor();
        ApplyHeadBob();
    }

    private void HandleRotation()
    {
        currentLookDelta = Vector2.SmoothDamp(currentLookDelta, lookInput, ref lookSmoothVelocity, smoothTime);
        
        transform.Rotate(Vector3.up * (currentLookDelta.x * sensitivity));

        xRotation -= (currentLookDelta.y * sensitivity);
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void SendMovementToMotor()
    {
        if (motorMovimiento == null) return;

        Vector3 worldDirection = (transform.forward * moveInput.y) + (transform.right * moveInput.x);
        motorMovimiento.SetMoveDirection(worldDirection);
    }

    private void ApplyHeadBob()
    {
        if (noiseComponent == null || motorMovimiento == null) return;

        if (motorMovimiento.IsGrounded && moveInput.magnitude > 0 && motorMovimiento.CurrentSpeed > 0.1f)
        {
            noiseComponent.AmplitudeGain = Mathf.Lerp(noiseComponent.AmplitudeGain, walkAmplitude, Time.deltaTime * 6f);
            noiseComponent.FrequencyGain = Mathf.Lerp(noiseComponent.FrequencyGain, walkFrequency, Time.deltaTime * 6f);
        }
        else
        {
            noiseComponent.AmplitudeGain = Mathf.Lerp(noiseComponent.AmplitudeGain, idleAmplitude, Time.deltaTime * 4f);
            noiseComponent.FrequencyGain = Mathf.Lerp(noiseComponent.FrequencyGain, 0.8f, Time.deltaTime * 4f);
        }
    }
}