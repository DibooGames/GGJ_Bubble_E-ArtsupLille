using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // Bulle que la caméra tourne autour
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float minY = -80f;
    [SerializeField] private float maxY = 80f;

    private Vector2 lookInput;
    private float yaw;
    private float pitch;

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void Start()    
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    private void LateUpdate()
    {
        // Calculate the camera rotation
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Debug.Log($"Yaw: {yaw}, Pitch: {pitch}");

        // Apply rotation
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        // Position the camera behind the target
        transform.position = target.position - transform.forward * 10f; // Adjust distance as needed
    }
}
