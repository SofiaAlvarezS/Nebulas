using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento Horizontal")]
    public float moveSpeed = 5f;

    [Header("Física Manual")]
    public float c1 = 32.66f;
    public float m = 120f;
    public float g = 9.8f;

    [Header("Salto")]
    public float jumpImpulse = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;       // Asignar en Inspector
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Animación")]
    public RuntimeAnimatorController animatorController;

    // Estado interno
    private float vy = 0f;
    private float y = 0f;
    private float moveInput = 0f;
    private bool jumpQueued = false;
    private bool isGrounded = false;

    // Componentes
    private Animator animator;

    void Awake()
    {
        // Guardamos altura inicial
        y = transform.position.y;

        // Animator
        animator = gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
    }

    void OnEnable()
    {
        InputService.OnMove += dir => moveInput = dir;
        InputService.OnJumpPressed += () => { if (isGrounded) jumpQueued = true; };
    }

    void OnDisable()
    {
        InputService.OnMove -= dir => moveInput = dir;
        InputService.OnJumpPressed -= () => { if (isGrounded) jumpQueued = true; };
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // 1) Detección manual de suelo
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        // 2) Movimiento horizontal por transform
        transform.position += Vector3.right * (moveInput * moveSpeed * dt);

        // 3) Salto
        if (jumpQueued)
        {
            vy = jumpImpulse;
            jumpQueued = false;
        }

        // 4) Física manual: gravedad + arrastre
        float dv = (-g - (c1 / m) * vy) * dt;
        vy += dv;

        // 5) Actualizar posición vertical
        y += vy * dt;
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;

        // 6) Animaciones
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
    }
}
