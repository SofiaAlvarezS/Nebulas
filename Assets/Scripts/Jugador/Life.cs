using System;
using System.Collections;
using UnityEngine;

public class Life : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float timeLostControl = 0.5f;
    [SerializeField] private float bouncePower = 5f;
    [SerializeField] private float damageFlashDuration = 0.5f;  // Duración total del parpadeo
    [SerializeField] private float flashInterval = 0.1f;        // Tiempo entre cambios de color

    [Header("UI de Vida")]
    [SerializeField] private Corazones corazones;

    private SpriteRenderer spriteRenderer;
    public event EventHandler playerDeath;

    private float currentHealth;
    private PlayerController2D playerController;
    private Animator animator;

    private void Awake()
    {
        currentHealth = maxHealth;

        playerController = GetComponent<PlayerController2D>();
        if (playerController == null)
            Debug.LogError("Life: falta PlayerController2D en el mismo GameObject.");

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Life: falta Animator en el mismo GameObject.");

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("Life: falta SpriteRenderer en el mismo GameObject.");

        if (corazones != null)
            corazones.UpdateHearts(currentHealth);
    }

    public void TakeDamage(float damage, Vector2 sourcePosition)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hit");

        if (playerController != null)
            playerController.Bounce(sourcePosition);

        if (playerController != null)
            StartCoroutine(LostControl());

        if (corazones != null)
            corazones.UpdateHearts(currentHealth);

        if (spriteRenderer != null)
            StartCoroutine(DamageFlash());

        if (currentHealth <= 0f)
        {
            playerDeath?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }

    private IEnumerator LostControl()
    {
        playerController.canMove = false;
        yield return new WaitForSeconds(timeLostControl);
        playerController.canMove = true;
    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer == null)
            yield break;

        Color originalColor = spriteRenderer.color;
        float elapsed = 0f;

        while (elapsed < damageFlashDuration)
        {
            // Cambiar a rojo
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashInterval);

            // Volver al color original
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2;
        }

        // Asegurarse que quede el color original
        spriteRenderer.color = originalColor;
    }
}
