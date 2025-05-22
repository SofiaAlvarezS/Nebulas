using UnityEngine;
using System.Collections;

/// <summary>
/// Enemigo que detecta colisiones manualmente con el jugador (sin usar Rigidbody/Colliders)
/// y permite que el jugador le golpee llamando a Hit().
/// </summary>
public class WolfScript : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [Tooltip("Vida inicial del enemigo")]
    public int maxHealth = 1;

    [Header("Daño al jugador")]
    [Tooltip("Daño que inflige al jugador al tocarlo")]
    public float damageToPlayer = 1f;
    [Tooltip("Vector de rebote aplicado al jugador")]
    public float bouncePower = 5f;
    [Tooltip("Cooldown en segundos entre daños consecutivos")]
    public float hitCooldown = 0.5f;

    private int currentHealth;
    private SpriteRenderer sprite;
    private float cooldownTimer;

    private PlayerController2D playerCtrl;
    private Life playerLife;

    void Awake()
    {
        currentHealth = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
            Debug.LogError("WolfScript: falta SpriteRenderer en el enemigo.");

        // Busca una referencia al Player por tag
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            playerCtrl = playerGO.GetComponent<PlayerController2D>();
            playerLife = playerGO.GetComponent<Life>();
            if (playerCtrl == null)
                Debug.LogError("WolfScript: PlayerController2D no encontrado en Player.");
            if (playerLife == null)
                Debug.LogError("WolfScript: Life no encontrado en Player.");
        }
        else
        {
            Debug.LogError("WolfScript: no se encontró GameObject con tag 'Player'.");
        }
    }

    void Update()
    {
        // Reduce el temporizador de cooldown
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        // Si el jugador existe y ya pasó el cooldown, comprueba colisión AABB
        if (cooldownTimer <= 0f && playerCtrl != null)
        {
            // Caja del enemigo
            Bounds myB = sprite.bounds;
            // Caja del jugador basada en PlayerController2D
            float px = playerCtrl.transform.position.x;
            float py = playerCtrl.transform.position.y;
            Vector2 half = playerCtrl.halfSize;
            var pMin = new Vector2(px - half.x, py - half.y);
            var pMax = new Vector2(px + half.x, py + half.y);

            // Chequea solapamiento
            bool overlapX = pMax.x > myB.min.x && pMin.x < myB.max.x;
            bool overlapY = pMax.y > myB.min.y && pMin.y < myB.max.y;
            if (overlapX && overlapY)
            {
                // Inflige daño y rebota al jugador
                playerLife.TakeDamage(damageToPlayer, transform.position);
                playerCtrl.Bounce(transform.position);
                cooldownTimer = hitCooldown;
            }
        }
    }

    /// <summary>
    /// Llamar desde el jugador (o proyectil) para hacerle daño al enemigo.
    /// </summary>
    public void Hit()
    {
        currentHealth--;
        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}
