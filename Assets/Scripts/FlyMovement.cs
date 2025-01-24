using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FlyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float ascendSpeed = 3f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private BoxCollider WinBox;

    private Vector2 moveInput;
    private bool isAscending;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GameManager.instance.isInDryArea = false;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dry"))
        {
            GameManager.instance.isInDryArea = true;
            GameManager.instance.UpdateIntegrity();
        }

        if(other.gameObject == WinBox.gameObject)
        {
            GameManager.instance.WinGame();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("dry"))
        {
            GameManager.instance.isInDryArea = false;
        }


    }

    private void FixedUpdate()
    {
        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        Vector3 moveDirection = flatForward * moveInput.y + cameraTransform.right * moveInput.x;
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = isAscending ? ascendSpeed : rb.velocity.y;

        rb.velocity = Vector3.MoveTowards(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }
}
