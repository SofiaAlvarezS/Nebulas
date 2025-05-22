
using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float jumpImpulse = 8f;

    [Header("Física Manual")]
    public float g = 20f;               // gravedad
    public float c1 = 2f;               // coeficiente aerodinámico
    public float m = 1f;                // masa
    public Vector2 halfSize = new Vector2(0.4f, 0.5f);

    [Header("Animaciones")]
    public RuntimeAnimatorController animatorController;

    [Header("Rebote y Control")]
    public bool canMove = true;
    public float bouncePower = 5f;

    [Header("Modificadores de Suelo")]
    [Tooltip("Velocidad en plataforma de jabón")]
    public float soapSpeedMultiplier = 1.5f;
    [Tooltip("Velocidad en plataforma de arena")]
    public float sandSpeed = 2f;

    private float x, y, vy;
    private bool isGrounded, jumpQueued;
    private Animator animator;
    private SpriteRenderer sprite;

    // Estructura para guardar rect y tag
    private struct PlatformData { public Rect rect; public string tag; }
    private List<PlatformData> platforms = new List<PlatformData>();

    private void Awake()
    {
        x = transform.position.x;
        y = transform.position.y;
        vy = 0f;

        animator = GetComponent<Animator>();
        if (animatorController != null)
            animator.runtimeAnimatorController = animatorController;

        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Reconstruir lista de plataformas dinámicas
        platforms.Clear();
        AddPlatformsByTag("Ground");
        AddPlatformsByTag("Jabon");
        AddPlatformsByTag("Arena");

        // Lectura de input
        float moveInput = canMove ? Input.GetAxisRaw("Horizontal") : 0f;
        if (Input.GetButtonDown("Jump") && isGrounded && canMove)
            jumpQueued = true;

        // 1) Física vertical manual
        vy += (-g - (c1 / m) * vy) * dt;
        y += vy * dt;

        // 2) Detección de suelo y ajuste de velocidad
        isGrounded = false;
        string currentPlatformTag = "";
        foreach (var p in platforms)
        {
            Vector2 pMin = new Vector2(x - halfSize.x, y - halfSize.y);
            Vector2 pMax = new Vector2(x + halfSize.x, y + halfSize.y);
            bool overlapX = pMax.x > p.rect.xMin && pMin.x < p.rect.xMax;
            bool overlapY = pMin.y < p.rect.yMax && pMin.y > p.rect.yMin;
            if (overlapX && overlapY && vy <= 0f)
            {
                y = p.rect.yMax + halfSize.y;
                vy = 0f;
                isGrounded = true;
                currentPlatformTag = p.tag;
                break;
            }
        }

        // 3) Movimiento horizontal con modificador según tipo de plataforma
        float actualSpeed = moveSpeed;
        if (isGrounded)
        {
            if (currentPlatformTag == "Jabon")
                actualSpeed *= soapSpeedMultiplier;
            else if (currentPlatformTag == "Arena")
                actualSpeed = sandSpeed;
        }
        x += moveInput * actualSpeed * dt;

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

    private void AddPlatformsByTag(string tag)
    {
        var floors = GameObject.FindGameObjectsWithTag(tag);
        foreach (var floor in floors)
        {
            var sr = floor.GetComponent<SpriteRenderer>();
            if (sr == null) continue;
            var b = sr.bounds;
            platforms.Add(new PlatformData { rect = new Rect(b.min.x, b.min.y, b.size.x, b.size.y), tag = tag });
        }
    }

    /// <summary>
    /// Rebota al jugador al recibir daño desde una posición de origen.
    /// </summary>
    public void Bounce(Vector2 sourcePosition)
    {
        float dir = (x - sourcePosition.x) >= 0f ? 1f : -1f;
        vy = bouncePower;
        x += dir * bouncePower * Time.deltaTime;
    }

    public void AddGroundRect(Rect r)
    {
        platforms.Add(new PlatformData { rect = r, tag = "Ground" });
    }
}
