using System;
using System.Collections;
using UnityEngine;

public class Life : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float timeLostControl = 0.5f;
    [SerializeField] private float bouncePower = 5f;

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
    }

    /// <summary>
    /// Llama cuando el jugador recibe daño.
    /// </summary>
    /// <param name="damage">Cantidad de vida a restar.</param>
    /// <param name="sourcePosition">Posición del origen del golpe (para rebotar).</param>
    public void TakeDamage(float damage, Vector2 sourcePosition)
    {
        currentHealth -= damage;

        // Disparamos la animación de golpe
        animator.SetTrigger("Hit");

        // Rebote inmediato
        if (playerController != null)
            playerController.Bounce(sourcePosition);


        // Pausar el control de movimiento
        if (playerController != null)
            StartCoroutine(LostControl());

        // Muerte
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
}
