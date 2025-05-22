using UnityEngine;
using System.Collections.Generic;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpImpulse = 8f;

    [Header("F�sica Manual")]
    public float g = 20f;               // gravedad
    public float c1 = 2f;                // coeficiente aerodin�mico
    public float m = 1f;                // masa
    public Vector2 halfSize = new Vector2(0.4f, 0.5f);

    [Header("Animaciones")]
    public RuntimeAnimatorController animatorController;

    [Header("Rebote y Control")]
    public bool canMove = true;                   // controla si el jugador responde al input
    public float bouncePower = 5f;                // fuerza del rebote tras da�o

    private float x, y, vy;
    private bool isGrounded, jumpQueued;
    private Animator animator;
    private SpriteRenderer sprite;
    private List<Rect> groundRects = new List<Rect>();

    private void Awake()
    {
        // Inicializa posici�n y velocidad
        x = transform.position.x;
        y = transform.position.y;
        vy = 0f;

        // Obtiene componentes
        animator = GetComponent<Animator>();
        if (animatorController != null)
            animator.runtimeAnimatorController = animatorController;

        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Actualizar lista de plataformas din�micamente
        groundRects.Clear();
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Ground");
        foreach (var floor in floors)
        {
            var sr = floor.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var b = sr.bounds;
                groundRects.Add(new Rect(b.min.x, b.min.y, b.size.x, b.size.y));
            }
        }

        // Lectura de input
        float moveInput = 0f;
        if (canMove)
            moveInput = Input.GetAxisRaw("Horizontal");  // -1, 0, +1

        if (Input.GetButtonDown("Jump") && isGrounded && canMove)
            jumpQueued = true;

        // --- Movimiento autom�tico si est� est�tico ---
        if (moveInput == 0f && isGrounded && !jumpQueued)
        {
            // Simular que el personaje es arrastrado hacia la izquierda
            x -= 5f * Time.deltaTime; // Puedes ajustar esta velocidad
        }

        // 1) Movimiento horizontal
        x += moveInput * moveSpeed * dt;

        // 2) F�sica vertical manual
        vy += (-g - (c1 / m) * vy) * dt;
        y += vy * dt;

        // 3) Detecci�n de suelo (AABB)
        isGrounded = false;
        Vector2 pMin = new Vector2(x - halfSize.x, y - halfSize.y);
        Vector2 pMax = new Vector2(x + halfSize.x, y + halfSize.y);
        foreach (var plat in groundRects)
        {
            bool overlapX = pMax.x > plat.xMin && pMin.x < plat.xMax;
            bool overlapY = pMin.y < plat.yMax && pMin.y > plat.yMin;
            if (overlapX && overlapY && vy <= 0f)
            {
                y = plat.yMax + halfSize.y;
                vy = 0f;
                isGrounded = true;
                break;
            }
        }

        // 4) Salto
        if (jumpQueued && isGrounded)
        {
            vy = jumpImpulse;
            jumpQueued = false;
        }

        // 5) Aplicar posici�n
        transform.position = new Vector3(x, y, transform.position.z);

        // 6) Animaciones
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsGrounded", isGrounded);
            bool isIdle = (moveInput == 0f) && isGrounded && !jumpQueued;
            animator.SetBool("IsIdle", isIdle);
        }

        // 7) Flip horizontal
        if (sprite != null)
        {
            if (moveInput > 0f) sprite.flipX = false;
            else if (moveInput < 0f) sprite.flipX = true;
        }
    }

    /// <summary>
    /// Rebota al jugador al recibir da�o desde una posici�n de origen.
    /// </summary>
    public void Bounce(Vector2 sourcePosition)
    {
        float dir = (x - sourcePosition.x) >= 0f ? 1f : -1f;
        vy = bouncePower;
        x += dir * bouncePower * Time.deltaTime;
    }

    public void AddGroundRect(Rect r)
    {
        groundRects.Add(r);
    }
}