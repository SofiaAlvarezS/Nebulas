using System;
using System.Collections;
using UnityEngine;

public class Life : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float timeLostControl = 0.5f;
    [SerializeField] private float damageFlashDuration = 0.5f;
    [SerializeField] private float flashInterval = 0.1f;

    [Header("UI de Vida")]
    [SerializeField] private Corazones corazones;

    [Header("Game Over")]
    [SerializeField] private GameOverManager gameOverManager;

    private SpriteRenderer spriteRenderer;
    public event EventHandler playerDeath;

    private float currentHealth;
    private PlayerController2D playerController;
    private Animator animator;

    private bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 1.0f;

    private void Awake()
    {
        currentHealth = maxHealth;

        playerController = GetComponent<PlayerController2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (corazones != null)
            corazones.UpdateHearts(currentHealth);
    }

    public void TakeDamage(float damage, Vector2 sourcePosition)
    {
        if (isInvulnerable)
            return;

        currentHealth -= damage;

        if (animator != null)
            animator.SetTrigger("Hit");

        if (playerController != null)
            playerController.Bounce(sourcePosition);

        if (playerController != null)
            StartCoroutine(LostControl());

        if (corazones != null)
            corazones.UpdateHearts(currentHealth);

        if (spriteRenderer != null)
            StartCoroutine(DamageFlash());

        StartCoroutine(InvulnerabilityTimer());

        if (currentHealth <= 0f)
        {
            playerDeath?.Invoke(this, EventArgs.Empty);
            if (gameOverManager != null)
                gameOverManager.ShowGameOver();

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
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flashInterval);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2;
        }

        spriteRenderer.color = originalColor;
    }

    private IEnumerator InvulnerabilityTimer()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }
}
