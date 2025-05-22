using UnityEngine;
using System.Collections.Generic;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpImpulse = 8f;
    public float g = 20f;
    public float c1 = 2f;
    public float m = 1f;

    [Header("Colisión Manual")]
    public Vector2 halfSize = new Vector2(0.4f, 0.5f);

    private float x;
    private float y;
    private float vy;
    private float moveInput;
    private bool jumpQueued;
    private bool isGrounded;

    private Animator animator;
    private SpriteRenderer sprite;

    // Lista de plataformas (hardcode o cargar dinámicamente)
    private static readonly List<Rect> groundRects = new List<Rect>
    {
        new Rect(-10f, -1f, 20f, 1f),
        new Rect(  5f,  2f,  3f, 1f),
        // añade tus plataformas...
    };

    private void Awake()
    {
        x = transform.position.x;
        y = transform.position.y;
        vy = 0f;

        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        InputService.OnMove += HandleMove;
        InputService.OnJumpPressed += HandleJump;
    }

    private void OnDisable()
    {
        InputService.OnMove -= HandleMove;
        InputService.OnJumpPressed -= HandleJump;
    }

    private void HandleMove(float dir)
    {
        moveInput = dir;
    }

    private void HandleJump()
    {
        if (isGrounded)
            jumpQueued = true;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // 1) Movimiento horizontal
        x += moveInput * moveSpeed * dt;

        // 2) Física vertical manual
        vy += (-g - (c1 / m) * vy) * dt;
        y += vy * dt;

        // 3) Detección de colisiones AABB con plataformas
        isGrounded = false;
        Vector2 pMin = new Vector2(x - halfSize.x, y - halfSize.y);
        Vector2 pMax = new Vector2(x + halfSize.x, y + halfSize.y);

        foreach (var plat in groundRects)
        {
            Vector2 oMin = new Vector2(plat.xMin, plat.yMin);
            Vector2 oMax = new Vector2(plat.xMax, plat.yMax);

            bool overlapX = pMax.x > oMin.x && pMin.x < oMax.x;
            bool overlapY = pMin.y < oMax.y && pMin.y > oMin.y;

            if (overlapX && overlapY && vy <= 0f)
            {
                y = plat.yMax + halfSize.y;
                vy = 0f;
                isGrounded = true;
                break;
            }
        }

        // 4) Salto pendiente
        if (jumpQueued && isGrounded)
        {
            vy = jumpImpulse;
            jumpQueued = false;
        }

        // 5) Aplicar posición
        transform.position = new Vector3(x, y, transform.position.z);

        // 6) Animaciones
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);

        // 7) Flip horizontal
        if (moveInput > 0) sprite.flipX = false;
        else if (moveInput < 0) sprite.flipX = true;
    }
}