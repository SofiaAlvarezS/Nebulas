using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    PauseController pauseCtrl;

    void Start()
    {
        pauseCtrl = Object.FindFirstObjectByType<PauseController>();
    }

    void OnMouseDown()
    {
        pauseCtrl.Resume();
    }
}
