using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    PauseController pauseCtrl;

    void Start()
    {
        pauseCtrl = FindObjectOfType<PauseController>();
    }

    void OnMouseDown()
    {
        pauseCtrl.Resume();
    }
}
