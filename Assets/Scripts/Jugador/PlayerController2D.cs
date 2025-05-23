using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TaggedRect
{
    public Rect rect;
    public string tag;

    public TaggedRect(Rect rect, string tag)
    {
        this.rect = rect;
        this.tag = tag;
    }
}

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
    public bool canMove = true;                   // controla si el jugador responde al input
    public float bouncePower = 5f;                // fuerza del rebote tras daño

    private float x, y, vy;
    private bool isGrounded, jumpQueued;
    private Animator animator;
    private SpriteRenderer sprite;
    private List<TaggedRect> groundRects = new List<TaggedRect>();
    private string currentGroundTag = "";

    private float residualHorizontalSpeed = 0f;  // velocidad horizontal acumulada para deslizamiento

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

        // Actualizar lista de plataformas
        groundRects.Clear();
        AddRectsFromObjects(GameObject.FindGameObjectsWithTag("Ground"), "Ground");
        AddRectsFromObjects(GameObject.FindGameObjectsWithTag("Ice"), "Ice");
        AddRectsFromObjects(GameObject.FindGameObjectsWithTag("Sand"), "Sand");

        // Lectura de input
        float moveInput = 0f;
        if (canMove)
            moveInput = Input.GetAxisRaw("Horizontal");  // -1, 0, +1

        if (Input.GetButtonDown("Jump") && isGrounded && canMove)
            jumpQueued = true;

        // Modificador de fricción por tag
        float speedMultiplier = 1f;
        if (isGrounded)
        {
            if (currentGroundTag == "Ice")
                speedMultiplier = 1.5f;
            else if (currentGroundTag == "Sand")
                speedMultiplier = 0.4f;
        }

        // Control de velocidad horizontal con deslizamiento y desplazamiento automático
        if (moveInput != 0f)
        {
            // Si hay input, velocidad residual se actualiza con input (izquierda duplica velocidad)
            float inputSpeed = moveInput;
            if (moveInput < 0f)
                inputSpeed *= 2f;

            residualHorizontalSpeed = inputSpeed * moveSpeed * speedMultiplier;
        }
        else
        {
            if (isGrounded)
            {
                float friction = 0f;
                float targetSpeed = -5f;

                if (currentGroundTag == "Ice")
                {
                    friction = 1.5f;  // bajo para deslizamiento suave
                }
                else if (currentGroundTag == "Sand")
                {
                    friction = 40f;   // fricción alta en arena
                }
                else // Ground normal
                {
                    friction = 20f;   // fricción media en suelo normal
                }

                residualHorizontalSpeed = Mathf.MoveTowards(residualHorizontalSpeed, targetSpeed, friction * dt);
            }
            else
            {
                // En aire reduce rápido velocidad residual para evitar deslizamiento
                float friction = 25f;
                residualHorizontalSpeed = Mathf.MoveTowards(residualHorizontalSpeed, 0f, friction * dt);
            }
        }

        // Aplicar movimiento horizontal
        x += residualHorizontalSpeed * dt;

        // Física vertical
        vy += (-g - (c1 / m) * vy) * dt;
        y += vy * dt;

        // Detección de suelo
        isGrounded = false;
        currentGroundTag = "";
        Vector2 pMin = new Vector2(x - halfSize.x, y - halfSize.y);
        Vector2 pMax = new Vector2(x + halfSize.x, y + halfSize.y);

        foreach (var plat in groundRects)
        {
            Rect r = plat.rect;
            bool overlapX = pMax.x > r.xMin && pMin.x < r.xMax;
            bool overlapY = pMin.y < r.yMax && pMin.y > r.yMin;
            if (overlapX && overlapY && vy <= 0f)
            {
                y = r.yMax + halfSize.y;
                vy = 0f;
                isGrounded = true;
                currentGroundTag = plat.tag;
                break;
            }
        }

        // Salto
        if (jumpQueued && isGrounded)
        {
            vy = jumpImpulse;
            jumpQueued = false;
        }

        // Aplicar posición
        transform.position = new Vector3(x, y, transform.position.z);

        // Animaciones
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(residualHorizontalSpeed));
            animator.SetBool("IsGrounded", isGrounded);
            bool isIdle = (moveInput == 0f) && isGrounded && !jumpQueued;
            animator.SetBool("IsIdle", isIdle);
        }

        // Flip sprite
        if (sprite != null)
        {
            if (moveInput > 0f) sprite.flipX = false;
            else if (moveInput < 0f) sprite.flipX = true;
        }
    }

    /// <summary>
    /// Añade rectángulos desde objetos con SpriteRenderer.
    /// </summary>
    private void AddRectsFromObjects(GameObject[] objects, string tag)
    {
        foreach (var obj in objects)
        {
            var sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var b = sr.bounds;
                Rect r = new Rect(b.min.x, b.min.y, b.size.x, b.size.y);
                groundRects.Add(new TaggedRect(r, tag));
            }
        }
    }

    /// <summary>
    /// Rebota al jugador desde una posición de origen.
    /// </summary>
    public void Bounce(Vector2 sourcePosition)
    {
        float dir = (x - sourcePosition.x) >= 0f ? 1f : -1f;
        vy = bouncePower;
        x += dir * bouncePower * Time.deltaTime;
    }

    public void AddGroundRect(Rect r)
    {
        groundRects.Add(new TaggedRect(r, "Ground"));
    }
}
