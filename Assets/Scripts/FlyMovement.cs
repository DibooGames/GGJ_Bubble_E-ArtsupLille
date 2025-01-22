using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FlyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float ascendSpeed = 3f;
    
    private Vector2 moveInput;
    private bool isAscending;
    private Rigidbody rb;
   [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
         // Assume the main camera is the player’s camera
    }

    public void SetCameraTransform(Transform camTransform)
    {
        cameraTransform = camTransform;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAscend(InputAction.CallbackContext context)
    {
        isAscending = context.ReadValueAsButton();
        Debug.Log($"Is Ascending: {isAscending}");
    }

    private void FixedUpdate()
    {
        // Ignore the camera’s vertical tilt by flattening its forward vector
        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        // Compute the move direction in the horizontal plane
        Vector3 moveDirection = flatForward * moveInput.y + cameraTransform.right * moveInput.x;

        // Apply movement
        Vector3 newVelocity = moveDirection * moveSpeed;
        if (isAscending)
        {
            newVelocity.y = ascendSpeed;
        }
        else
        {
            newVelocity.y = rb.velocity.y; // Keep the current vertical velocity
        }
        rb.velocity = newVelocity;
    }
}