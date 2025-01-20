using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class BulleMovement : MonoBehaviour
{
    //--- Actions depuis le nouvel Input System ---
    private InputActionMap playerActionMap;
    private InputAction accelDecelAction;   // -1, 0, +1
    private InputAction floatAction;        // bool (button)
    private InputAction viewAction;         // Vector2 pour la souris / stick

    //--- Contrôle de la bulle ---
    [Header("Paramètres de déplacement")]
    [Tooltip("Facteur d'accélération quand l'input est ±1.")]
    public float acceleration = 5f;
    
    [Tooltip("Vitesse maximale le long de l'axe Y local.")]
    public float maxSpeed = 10f;
    
    [Tooltip("Décélération quand l'input revient à 0.")]
    public float deceleration = 5f;

    [Tooltip("Force supplémentaire appliquée en haut quand 'Float' est maintenu (ex: Espace).")]
    public float floatForce = 5f;

    //--- Contrôle de la caméra ---
    [Header("Paramètres de caméra")]
    [Tooltip("Pivot ou GameObject à faire pivoter pour la caméra (parent de la Camera).")]
    public Transform cameraPivot;

    [Tooltip("Sensibilité de rotation de la caméra (°/sec par unité d'input).")]
    public float cameraSensitivity = 180f;

    [Tooltip("Limite (en degrés) de l'angle de pitch (haut/bas).")]
    public float pitchClamp = 80f;

    //--- États internes ---
    private Rigidbody rb;
    private float accelInput = 0f;         // Valeur brute de "Acceleration/decceleration"
    private float currentSpeed = 0f;       // Vitesse actuelle le long de Y local
    private bool isFloatPressed = false;   // Est-ce qu'on maintient la touche Float ?

    // Variables pour la rotation de caméra
    private float cameraPitch = 0f; // rotation autour de X
    private float cameraYaw = 0f;   // rotation autour de Y

    private void Start()
    {
        // 1) Récupération de l'Action Map "Player"
        var playerInput = GetComponent<PlayerInput>();
        playerActionMap = playerInput.actions.FindActionMap("Player");
        if (playerActionMap == null)
        {
            Debug.LogError("Impossible de trouver l'Action Map 'Player'.");
            return;
        }

        // 2) Récupérer les différentes actions
        accelDecelAction = playerActionMap.FindAction("Acceleration/decceleration");
        floatAction      = playerActionMap.FindAction("Float");
        viewAction       = playerActionMap.FindAction("View");

        if (accelDecelAction == null) Debug.LogError("'Acceleration/decceleration' introuvable.");
        if (floatAction == null)      Debug.LogError("'Float' introuvable.");
        if (viewAction == null)       Debug.LogError("'View' introuvable.");

        // 3) Souscrire aux événements
        if (accelDecelAction != null)
        {
            accelDecelAction.performed += ctx => { accelInput = ctx.ReadValue<float>(); };
            accelDecelAction.canceled  += ctx => { accelInput = 0f; };
        }

        if (floatAction != null)
        {
            floatAction.performed += ctx => { isFloatPressed = true; };
            floatAction.canceled  += ctx => { isFloatPressed = false; };
        }

        // Pas d'événements spécifiques pour viewAction : on fera un ReadValue dans Update()

        // 4) Configuration du Rigidbody
        rb = GetComponent<Rigidbody>();
        // On NE désactive PAS la gravité => rb.useGravity = true par défaut

        // 5) Si aucun pivot caméra n'a été assigné, on prend ce transform
        if (cameraPivot == null)
        {
            cameraPivot = this.transform;
        }
    }

    private void Update()
    {
        // --- A) Contrôle de la caméra ---
        if (viewAction != null)
        {
            // Lire la valeur du "View" (Vector2) : delta souris / stick
            Vector2 viewDelta = viewAction.ReadValue<Vector2>();

            // On calcule l'angle à ajouter, en tenant compte de la sensibilité et du deltaTime
            float mouseX = viewDelta.x * (cameraSensitivity * 0.50f) * Time.deltaTime;
            float mouseY = viewDelta.y * (cameraSensitivity * 0.50f) * Time.deltaTime;

            // Mettre à jour le yaw/pitch
            cameraYaw   += mouseX;
            cameraPitch -= mouseY; // le "-" fait que la souris vers le haut => vise vers le haut
            cameraPitch  = Mathf.Clamp(cameraPitch, -pitchClamp, pitchClamp);

            // Appliquer la rotation au pivot (pitch autour de X, yaw autour de Y)
            cameraPivot.localRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        }
    }

    private void FixedUpdate()
    {
        // --- B) Calcul de la vitesse le long de l'axe Y local de la bulle ---
        if (!Mathf.Approximately(accelInput, 0f))
        {
            // On accélère ou recule suivant le signe de accelInput
            currentSpeed += accelInput * acceleration * Time.fixedDeltaTime;
            // Borne la vitesse
            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
        }
        else
        {
            // Décélération quand on n'appuie plus
            if (currentSpeed > 0f)
            {
                currentSpeed -= deceleration * Time.fixedDeltaTime;
                if (currentSpeed < 0f) currentSpeed = 0f;
            }
            else if (currentSpeed < 0f)
            {
                currentSpeed += deceleration * Time.fixedDeltaTime;
                if (currentSpeed > 0f) currentSpeed = 0f;
            }
        }

        // --- C) Construire la velocity dans l'espace monde ---
        // Le "forward" de la bulle est son axe Y local => transform.up
        // Donc on bouge sur XZ selon (transform.up * currentSpeed),
        // mais on conserve la composante verticale (y) due à la gravité.
        Vector3 localYMovement = transform.up * currentSpeed;
        float yVelocity = rb.velocity.y; // conserver la vitesse verticale (gravité)

        // Velocity finale
        Vector3 finalVelocity = new Vector3(localYMovement.x, yVelocity, localYMovement.z);
        rb.velocity = finalVelocity;

        // --- D) Gérer la touche "Float" ---
        // Quand on maintient Float (ex: Espace), on applique une force vers le haut
        if (isFloatPressed)
        {
            // On applique une force "vers le haut" pour contrer (partiellement ou totalement) la gravité
            // ForceMode.Acceleration => force indépendante de la masse
            rb.AddForce(Vector3.up * floatForce, ForceMode.Acceleration);
        }
    }
}
