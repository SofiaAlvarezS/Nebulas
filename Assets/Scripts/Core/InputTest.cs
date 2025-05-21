using UnityEngine;

public class InputTest : MonoBehaviour
{
    private void OnEnable()
    {
        InputService.OnMove += dir => Debug.Log($"[Test] Move: {dir}");
        InputService.OnJumpPressed += () => Debug.Log("[Test] Jump Pressed");
        InputService.OnJumpReleased += () => Debug.Log("[Test] Jump Released");
    }

    private void OnDisable()
    {
        InputService.OnMove -= dir => Debug.Log($"[Test] Move: {dir}");
        InputService.OnJumpPressed -= () => Debug.Log("[Test] Jump Pressed");
        InputService.OnJumpReleased -= () => Debug.Log("[Test] Jump Released");
    }
}
