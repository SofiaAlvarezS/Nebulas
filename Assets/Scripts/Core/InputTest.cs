using UnityEngine;

public class InputTest : MonoBehaviour
{
    private void OnEnable()
    {
        InputService.OnMove += HandleMove;
        InputService.OnJumpPressed += HandleJump;
    }

    private void OnDisable()
    {
        InputService.OnMove -= HandleMove;
        InputService.OnJumpPressed -= HandleJump;
    }

    private void HandleMove(float dir)
    {
        Debug.Log($"[Test] Move: {dir}");
    }

    private void HandleJump()
    {
        Debug.Log("[Test] Jump pressed");
    }
}
