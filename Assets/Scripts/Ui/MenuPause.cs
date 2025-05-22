using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{

    [SerializeField] private GameObject bottomPause;

    [SerializeField] private GameObject menuPause;

    private bool gamePause;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (gamePause)
            {
                resume();  
            }
            else
            {
                Pause();    
            }
        }
    }
    public void Pause()
    {
        gamePause = true;
        Time.timeScale = 0f;
        bottomPause.SetActive(false);
        menuPause.SetActive(true);
    }

    public void resume()
    {
        Time.timeScale = 1f;
        bottomPause.SetActive(true);
        menuPause.SetActive(false);
    }

    public void replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void mainMenu(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void quit()
    {
        Application.Quit();
    }
}
