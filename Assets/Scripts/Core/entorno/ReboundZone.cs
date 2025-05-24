using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public Vector2 halfSize = new Vector2(2.81f, 9.26f);
    public Vector2 reboundForce = new Vector2(20f, 10f);

    private PlayerController2D player;
    private Life playerLife;

    private void Start()
    {
        player = UnityEngine.Object.FindFirstObjectByType<PlayerController2D>();
        playerLife = UnityEngine.Object.FindFirstObjectByType<Life>();

        if (player == null)
        {
            Debug.LogWarning("PlayerController2D no encontrado en la escena.");
        }
        if (playerLife == null)
        {
            Debug.LogWarning("Life no encontrado en la escena.");
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        Vector2 pPos = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 zPos = new Vector2(transform.position.x, transform.position.y);

        bool overlapX = Mathf.Abs(pPos.x - zPos.x) < (player.halfSize.x + halfSize.x);
        bool overlapY = Mathf.Abs(pPos.y - zPos.y) < (player.halfSize.y + halfSize.y);

        if (overlapX && overlapY)
        {
            Debug.Log("Jugador dentro de zona de daño - aplicando rebote");

            if (playerLife != null)
            {
                playerLife.TakeDamage(1f, transform.position);
                player.ApplyRebound(reboundForce);  // Rebote fuerte con vector definido
            }
            else
            {
                player.ApplyRebound(reboundForce);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, halfSize * 2);
    }
}
