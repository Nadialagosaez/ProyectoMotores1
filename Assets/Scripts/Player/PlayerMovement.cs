using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 7f; 
    [SerializeField] private float jumpHeight = 1.8f;
    [SerializeField] private float extraGravity = 20f; 

    [Header("Ground Check")]
    [SerializeField] private float checkRadius = 0.25f;
    [SerializeField] private LayerMask groundMask;      

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Vector3 moveDirection;
    private bool isGrounded;

    public bool IsGrounded => isGrounded;
    public float CurrentSpeed => new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        
        rb.freezeRotation = true; 
        rb.useGravity = true;
    }

    void Update()
    {
        Vector3 feetPosition = GetFeetPosition();
        
        isGrounded = Physics.CheckSphere(feetPosition, checkRadius, groundMask);
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    public void Jump()
    {
        if (isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }
    }

    private void ApplyMovement()
    {
        Vector3 targetVelocity = moveDirection * speed;
        float currentYVelocity = rb.linearVelocity.y;

        if (moveDirection.magnitude == 0)
        {
            rb.linearVelocity = new Vector3(0, currentYVelocity, 0);
        }
        else
        {
            rb.linearVelocity = new Vector3(targetVelocity.x, currentYVelocity, targetVelocity.z);
        }

        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
    }

    private Vector3 GetFeetPosition()
    {
        if (capsule == null) return transform.position;
        // Tomamos el centro de la cápsula y restamos la mitad de su altura
        return transform.position + capsule.center + Vector3.down * (capsule.height / 2f);
    }

    // DIBUJO EN EL EDITOR
    private void OnDrawGizmosSelected()
    {
        if (capsule == null) return;
        
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(GetFeetPosition(), checkRadius);
    }
}