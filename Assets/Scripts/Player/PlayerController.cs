using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Humanoid Rotation")]
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 80f;
    [Range(0.01f, 0.5f)] [SerializeField] private float smoothTime = 0.05f;

    [Header("Walk (Head Bob)")]
    [SerializeField] private float bobFrequency = 5f; // Velocidad del paso
    [SerializeField] private float bobAmount = 0.05f;    // Altura del balanceo
    private float bobTimer = 0f;
    private float defaultCameraY;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float xRotation = 0f;
    private Vector2 currentLookDelta;
    private Vector2 lookSmoothVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        // altura original de la cámara
        defaultCameraY = cameraTransform.localPosition.y;
    }

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnRotate(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
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

    private void HandleMovement()
    {
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;

        Vector3 move = (transform.forward * moveInput.y) + (transform.right * moveInput.x);
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void ApplyHeadBob()
    {
        
        if (!controller.isGrounded || moveInput.magnitude == 0)
        {
            
            bobTimer = 0;
            Vector3 newPos = new Vector3(cameraTransform.localPosition.x, Mathf.Lerp(cameraTransform.localPosition.y, defaultCameraY, Time.deltaTime * 10f), cameraTransform.localPosition.z);
            cameraTransform.localPosition = newPos;
            return;
        }

        //oscilacion
        bobTimer += Time.deltaTime * bobFrequency;
        
        float bobOffsetY = Mathf.Sin(bobTimer) * bobAmount;
        
        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            defaultCameraY + bobOffsetY,
            cameraTransform.localPosition.z
        );
    }
}