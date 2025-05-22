using UnityEngine;
using System;

public static class InputService
{
    public static event Action<float> OnMove;
    public static event Action OnJumpPressed;

    public static void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");   // -1, 0, 1
        OnMove?.Invoke(h);

        if (Input.GetButtonDown("Jump"))
            OnJumpPressed?.Invoke();
    }
}
