using UnityEngine;
using System.Collections.Generic;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpImpulse = 8f;

    [Header("Física Manual")]
    public float g = 20f;               // gravedad
    public float c1 = 2f;                // coeficiente aerodinámico
    public float m = 1f;                // masa
    public Vector2 halfSize = new Vector2(0.4f, 0.5f);

    [Header("Animaciones")]
    public RuntimeAnimatorController animatorController;

    [Header("Rebote y Control")]
    public bool canMove = true;                   // controla si el jugador responde al input
    public float bouncePower = 5f;                // fuerza del rebote tras daño

    private float x, y, vy;
    private bool isGrounded, jumpQueued;
    private Animator animator;
    private SpriteRenderer sprite;
    private List<Rect> groundRects;

    private void Awake()
    {
        // Inicializa posición y velocidad
        x = transform.position.x;
        y = transform.position.y;
        vy = 0f;

        // Obtiene componentes
        animator = GetComponent<Animator>();
        if (animatorController != null)
            animator.runtimeAnimatorController = animatorController;

        sprite = GetComponent<SpriteRenderer>();

        // Carga plataformas tagged "Ground"
        groundRects = new List<Rect>();
        try
        {
            foreach (var floor in GameObject.FindGameObjectsWithTag("Ground"))
            {
                var sr = floor.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    var b = sr.bounds;
                    groundRects.Add(new Rect(b.min.x, b.min.y, b.size.x, b.size.y));
                }
            }
        }
        catch
        {
            Debug.LogWarning("Tag 'Ground' no definida. Crea la Tag y aplícala al suelo.");
        }

        Debug.Log($"[PlayerController2D] Plataformas detectadas: {groundRects.Count}");
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Lectura de input
        float moveInput = 0f;
        if (canMove)
            moveInput = Input.GetAxisRaw("Horizontal");  // -1, 0, +1

        if (Input.GetButtonDown("Jump") && isGrounded && canMove)
            jumpQueued = true;

        // 1) Movimiento horizontal
        x += moveInput * moveSpeed * dt;

        // 2) Física vertical manual
        vy += (-g - (c1 / m) * vy) * dt;
        y += vy * dt;

        // 3) Detección de suelo (AABB)
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

        // 5) Aplicar posición
        transform.position = new Vector3(x, y, transform.position.z);

        // 6) Animaciones
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsGrounded", isGrounded);
        }

        // 7) Flip horizontal
        if (sprite != null)
        {
            if (moveInput > 0f) sprite.flipX = false;
            else if (moveInput < 0f) sprite.flipX = true;
        }
    }

    /// <summary>
    /// Rebota al jugador al recibir daño desde una posición de origen.
    /// </summary>
    public void Bounce(Vector2 sourcePosition)
    {
        // Dirección horizontal inversa al golpe
        float dir = (x - sourcePosition.x) >= 0f ? 1f : -1f;
        // Rebote vertical e impulso lateral
        vy = bouncePower;
        x += dir * bouncePower * Time.deltaTime;
    }
}
