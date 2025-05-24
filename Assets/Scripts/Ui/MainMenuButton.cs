using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    PauseController pauseCtrl;

    void Start()
    {
        pauseCtrl = FindObjectOfType<PauseController>();
    }

    void OnMouseDown()
    {
        pauseCtrl.GoToMainMenu();
    }
}
