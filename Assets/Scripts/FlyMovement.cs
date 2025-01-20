using UnityEngine;
using UnityEngine.InputSystem;


public class BulleMovement : MonoBehaviour
{
    //--- Actions depuis le nouvel Input System ---
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction viewAction;
    private InputAction floatAction;

    //--- Variables de mouvement ---
    [Header("Mouvement de la bulle")]
    public float speed = 5f;       // Vitesse de déplacement horizontal
    public float floatForce = 5f;  // Force vers le haut en maintenant Espace

    //--- Variables de rotation de la caméra ---
    [Header("Rotation caméra")]
    public float mouseSensitivity = 100f;   // Sensibilité de la souris
    public float maxVerticalAngle = 80f;    // Limite d'angle vertical (en degrés)

    // Pivot ou objet caméra enfant (pour gérer le pitch)
    // Assignez-le depuis l’inspecteur (par ex. la Main Camera en enfant).
    public Transform cameraPivot;

    //--- Interne ---
    private Rigidbody rb;
    private float pitch = 0f;   // Stockage de l'angle vertical

    private void Awake()
    {
        // Récupération du Rigidbody (gravité activée dans l’Inspector)
        rb = GetComponentInChildren<Rigidbody>();
        
        // Récupération du PlayerInput et des actions
        var playerInput = GetComponent<PlayerInput>();
        playerActionMap = playerInput.actions.FindActionMap("Player");

        moveAction   = playerActionMap.FindAction("Move");
        viewAction   = playerActionMap.FindAction("View");
        floatAction  = playerActionMap.FindAction("Float");
    }

    private void OnEnable()
    {
        // Activation des actions
        moveAction.Enable();
        viewAction.Enable();
        floatAction.Enable();
    }

    private void OnDisable()
    {
        // Désactivation des actions
        moveAction.Disable();
        viewAction.Disable();
        floatAction.Disable();
    }

    private void Update()
    {
        // =============================================================
        // 1) Gérer la rotation de la caméra (yaw + pitch) dans Update
        // =============================================================
        Vector2 viewInput = viewAction.ReadValue<Vector2>();

        // Rotation horizontale (yaw) : faire pivoter tout l'objet
        float yaw = viewInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, yaw);

        // Rotation verticale (pitch) : uniquement sur le pivot caméra
        float pitchDelta = -viewInput.y * mouseSensitivity * Time.deltaTime;
        pitch += pitchDelta;
        pitch = Mathf.Clamp(pitch, -maxVerticalAngle, maxVerticalAngle);

        if (cameraPivot != null)
        {
            cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        // =============================================================
        // 2) Gérer le déplacement physique dans FixedUpdate (Rigidbody)
        // =============================================================

        // Récupération des axes de déplacement (ZQSD/WASD) en 2D
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // On construit la direction voulue en se basant sur l'orientation actuelle
        // de l'objet (transform) : forward et right. 
        // On ignore la composante verticale pour le déplacement horizontal.
        Vector3 desiredDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        desiredDirection.y = 0f; // Empêcher la bulle de pencher vers le haut/bas sur ZQSD

        // On applique la vitesse en X et Z, tout en conservant la vitesse verticale
        Vector3 currentVelocity = rb.velocity;
        Vector3 horizontalVelocity = desiredDirection * speed;
        Vector3 newVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);

        rb.velocity = newVelocity;

        // Si la touche "Float" (Espace) est pressée, on applique une force vers le haut
        if (floatAction.IsPressed())
        {
            // On applique une force continue vers le haut
            rb.AddForce(Vector3.up * floatForce, ForceMode.Force);
        }

        // La gravité Unity s’occupe de faire retomber la bulle naturellement 
        // (Rigidbody.useGravity = true dans l’Inspector).
    }
}
