using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    PauseController pauseCtrl;

    void Start()
    {
        pauseCtrl = Object.FindFirstObjectByType<PauseController>();
    }

    void OnMouseDown()
    {
        pauseCtrl.GoToMainMenu();
    }
}
