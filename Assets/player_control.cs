using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController3D : MonoBehaviour
{
    Rigidbody rb;

    public InputSystem_Actions actions;

    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpForce = 7f;

    Vector2 moveInput;

    bool isGrounded = true;
    bool isRunning = false;

    void Awake()
    {
        actions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        actions.Player.Enable();

        actions.Player.Move.performed += Movement;
        actions.Player.Move.canceled += Movement;

        actions.Player.Jump.performed += Jumping;

        actions.Player.Run.performed += Running;
        actions.Player.Run.canceled += Running;
    }

    void OnDisable()
    {
        actions.Player.Disable();

        actions.Player.Move.performed -= Movement;
        actions.Player.Move.canceled -= Movement;

        actions.Player.Jump.performed -= Jumping;

        actions.Player.Run.performed -= Running;
        actions.Player.Run.canceled -= Running;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Movement(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void Running(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            isRunning = true;

        if (ctx.canceled)
            isRunning = false;
    }

    void Jumping(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpForce,
                rb.linearVelocity.z
            );

            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 movement = new Vector3(
            moveInput.x,
            0,
            moveInput.y
        );

        rb.linearVelocity = new Vector3(
            movement.x * currentSpeed,
            rb.linearVelocity.y,
            movement.z * currentSpeed
        );

        // Rotar personaje hacia donde camina
        if (movement != Vector3.zero)
        {
            transform.forward = movement;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }
}
}