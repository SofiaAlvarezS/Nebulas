using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string mainSceneName = "MainScene";

    private void OnMouseDown()
    {
        // Detecta clic/tap sobre el collider 2D
        SceneManager.LoadScene(mainSceneName);
    }
}
