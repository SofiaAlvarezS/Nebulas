using System;
using UnityEngine;

public static class InputService
{
    // Eventos para otros sistemas
    public static event Action<float> OnMove;      // valor de -1 a +1
    public static event Action OnJumpPressed;
    public static event Action OnJumpReleased;

    // Llamar a esto desde un MonoBehaviour en cada frame
    public static void Update()
    {
        // Movimiento horizontal
        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) > 0)
            OnMove?.Invoke(h);

        // Salto (botón configurado en ProjectSettings/InputManager)
        if (Input.GetButtonDown("Jump"))
            OnJumpPressed?.Invoke();
        if (Input.GetButtonUp("Jump"))
            OnJumpReleased?.Invoke();
    }
}
