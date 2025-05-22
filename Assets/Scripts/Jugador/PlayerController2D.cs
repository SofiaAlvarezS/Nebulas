using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
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
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Animación")]
    [Tooltip("El Animator Controller con tus clips Idle/Corrers/Jump")]
    public RuntimeAnimatorController animatorController;

    // Estado interno
    private float vy;
    private float y;
    private float moveInput;
    private bool jumpQueued;
    private bool isGrounded;

    // Componentes
    private Transform groundCheck;
    private Animator animator;

    void Awake()
    {
        // Collider
        var playerCollider = gameObject.AddComponent<BoxCollider2D>();
        playerCollider.offset = Vector2.zero;
        playerCollider.size = new Vector2(1f, 2f);

        // Rigidbody Kinematic (si lo necesitas)
        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // GroundCheck
        groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.parent = transform;
        groundCheck.localPosition = new Vector3(0f, -1f, 0f);
        var gcCol = groundCheck.gameObject.AddComponent<CircleCollider2D>();
        gcCol.isTrigger = true;
        gcCol.radius = groundRadius;

        // Posición inicial Y
        y = transform.position.y;

        // Animator
        animator = gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
    }

    void OnEnable()
    {
        InputService.OnMove += HandleMove;
        InputService.OnJumpPressed += HandleJumpPressed;
    }

    void OnDisable()
    {
        InputService.OnMove -= HandleMove;
        InputService.OnJumpPressed -= HandleJumpPressed;
    }

    private void HandleMove(float dir)
    {
        moveInput = dir;
    }

    private void HandleJumpPressed()
    {
        if (isGrounded)
            jumpQueued = true;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Chequeo suelo
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, groundRadius, groundLayer);

        // Movimiento horizontal
        transform.position += Vector3.right * (moveInput * moveSpeed * dt);

        // Cola de salto
        if (jumpQueued)
        {
            vy = jumpImpulse;
            jumpQueued = false;
        }

        // Física manual: gravedad + arrastre
        float dv = (-g - (c1 / m) * vy) * dt;
        vy += dv;

        // Actualizar Y
        y += vy * dt;
        var pos = transform.position;
        pos.y = y;
        transform.position = pos;

        // ———> Actualizar animaciones <———
        // Parámetro Speed (valor absoluto)
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        // Parámetro IsGrounded
        animator.SetBool("IsGrounded", isGrounded);
    }
}
