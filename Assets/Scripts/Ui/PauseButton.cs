using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private PauseController pauseCtrl;  // referencia assignable
    private Collider2D col2D;

    void Awake()
    {
        col2D = GetComponent<Collider2D>();
    }

    void OnMouseDown()
    {
        pauseCtrl.TogglePause();
    }
}