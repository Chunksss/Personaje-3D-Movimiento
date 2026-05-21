using UnityEngine;
using UnityEngine.InputSystem;

public class CONTROL : MonoBehaviour
{
    // =========================================================
    // COMPONENTES
    // =========================================================

    private Rigidbody rb;
    private Animator animator;

    // =========================================================
    // INPUT SYSTEM
    // =========================================================

    private InputSystem_Actions actions;

    // =========================================================
    // VARIABLES
    // =========================================================

    private Vector2 inputMovimiento;
    private bool estaCorriendo;
    private bool isGrounded;
    private bool estaHaciendoPose;

    // =========================================================
    // CONFIGURACIÓN
    // =========================================================

    [Header("Movimiento")]

    public float walkSpeed = 3f;
    public float runningSpeed = 7f;
    public float jumpForce = 5f;
    public float rotationSpeed = 15f;

    // =========================================================
    // ANIMATOR
    // =========================================================

    private float smoothVelocity;

    // =========================================================
    // AWAKE
    // =========================================================

    void Awake()
    {
        Debug.Log("SCRIPT FUNCIONANDO");

        // Crear Input System
        actions = new InputSystem_Actions();

        // Obtener componentes
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Configuración Rigidbody
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    // =========================================================
    // ON ENABLE
    // =========================================================

    void OnEnable()
    {
        if (actions == null)
        {
            actions = new InputSystem_Actions();
        }

        actions.Player.Enable();

        // MOVIMIENTO
        actions.Player.Move.performed += Movement;
        actions.Player.Move.canceled += Movement;

        // SALTO
        actions.Player.Jump.performed += Jumping;

        // CORRER
        actions.Player.Sprint.performed += SprintStart;
        actions.Player.Sprint.canceled += SprintStop;

        // INTERACT / POSE (Tecla E)
        actions.Player.Interact.performed += PoseStart;
        actions.Player.Interact.canceled += PoseStop;

        Debug.Log("INPUT ACTIVADO");
    }

    // =========================================================
    // ON DISABLE
    // =========================================================

    void OnDisable()
    {
        actions.Player.Move.performed -= Movement;
        actions.Player.Move.canceled -= Movement;

        actions.Player.Jump.performed -= Jumping;

        actions.Player.Sprint.performed -= SprintStart;
        actions.Player.Sprint.canceled -= SprintStop;

        actions.Player.Interact.performed -= PoseStart;
        actions.Player.Interact.canceled -= PoseStop;

        actions.Player.Disable();
    }

    // =========================================================
    // MOVIMIENTO INPUT
    // =========================================================

    void Movement(InputAction.CallbackContext ctx)
    {
        inputMovimiento = ctx.ReadValue<Vector2>();
        Debug.Log("Movimiento: " + inputMovimiento);
    }

    // =========================================================
    // SPRINT INPUT
    // =========================================================

    void SprintStart(InputAction.CallbackContext ctx)
    {
        estaCorriendo = true;
        Debug.Log("Corriendo");
    }

    void SprintStop(InputAction.CallbackContext ctx)
    {
        estaCorriendo = false;
    }

    // =========================================================
    // SALTO INPUT
    // =========================================================

    void Jumping(InputAction.CallbackContext ctx)
    {
        Debug.Log("Intento de salto");

        if (isGrounded && !estaHaciendoPose)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpForce,
                rb.linearVelocity.z
            );

            animator.SetTrigger("Jump");
            isGrounded = false;
            Debug.Log("Saltando");
        }
    }

    // =========================================================
    // INTERACT / POSE INPUT
    // =========================================================

    void PoseStart(InputAction.CallbackContext ctx)
    {
        estaHaciendoPose = true;
        animator.SetBool("IsPosing", true);
        Debug.Log("Inicia Pose (E presionada)");
    }

    void PoseStop(InputAction.CallbackContext ctx)
    {
        estaHaciendoPose = false;
        animator.SetBool("IsPosing", false);
        Debug.Log("Termina Pose (E soltada)");
    }

    // =========================================================
    // FIXED UPDATE
    // =========================================================

    void FixedUpdate()
    {
        Vector3 direccionMovimiento = new Vector3(
            inputMovimiento.x,
            0f,
            inputMovimiento.y
        );

        direccionMovimiento.Normalize();

        float velocidadActual = estaCorriendo ? runningSpeed : walkSpeed;
        if (estaHaciendoPose) velocidadActual = 0f;

        Vector3 velocidadFinal = direccionMovimiento * velocidadActual;

        rb.linearVelocity = new Vector3(
            velocidadFinal.x,
            rb.linearVelocity.y,
            velocidadFinal.z
        );

        // =====================================================
        // ROTACIÓN
        // =====================================================

        if (direccionMovimiento.magnitude > 0.1f && !estaHaciendoPose)
        {
            Quaternion rotacionObjetivo =
                Quaternion.LookRotation(direccionMovimiento);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        // =====================================================
        // ANIMATOR
        // =====================================================

        float velocidadHorizontal = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        ).magnitude;

        smoothVelocity = Mathf.MoveTowards(
            smoothVelocity,
            velocidadHorizontal,
            5f * Time.fixedDeltaTime
        );

        animator.SetFloat("Speed", smoothVelocity);
    }

    // =========================================================
    // DETECCIÓN DE SUELO
    // =========================================================

    private void OnCollisionEnter(Collision collision)
    {
        RevisarSuelo(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        RevisarSuelo(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    // =========================================================
    // REVISAR SUELO
    // =========================================================

    void RevisarSuelo(Collision collision)
    {
        foreach (ContactPoint contacto in collision.contacts)
        {
            if (contacto.normal.y > 0.6f)
            {
                isGrounded = true;
                return;
            }
        }
    }
}