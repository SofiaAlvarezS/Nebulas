using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuración de escenas")]
    [Tooltip("Nombre de la escena de menú principal")]
    public string mainMenuSceneName = "MainScene";
    [Tooltip("Nombre de la escena de gameplay")]
    public string gameplaySceneName = "MainScene";
    // Si menú y gameplay comparten MainScene, mantenlo igual (pero separa flags)

    [Header("Controles de inicio")]
    public KeyCode startKey = KeyCode.Space;
    public bool waitForAnyKey = false;

    private bool isPaused;
    public bool IsPaused => isPaused;

    private bool hasStartedGameplay;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName)
        {
            // RESET cuando vuelves al menú principal
            hasStartedGameplay = false;
            isPaused = true;
            Time.timeScale = 0f;
            AudioListener.pause = false;
        }
        else if (scene.name == gameplaySceneName)
        {
            // Al entrar a gameplay por primera vez:
            // dejamos isPaused = true para que Update espere input
            isPaused = true;
            Time.timeScale = 0f;
            AudioListener.pause = false;
        }
    }

    private void Update()
    {
        // Si estamos en “modo menú” dentro de gameplaySceneName
        if (!hasStartedGameplay)
        {
            bool started = waitForAnyKey
                ? Input.anyKeyDown
                : Input.GetKeyDown(startKey);

            if (started)
            {
                hasStartedGameplay = true;
                isPaused = false;
                Time.timeScale = 1f;

                // Arranca la música
                var mp = Object.FindFirstObjectByType<MusicPlayer>();
                if (mp != null) mp.Play();
            }
            return;
        }

        // Una vez arrancado, chequeo fin de pista
        if (!isPaused)
        {
            var mp = Object.FindFirstObjectByType<MusicPlayer>();
            if (mp != null && !mp.IsPlaying && mp.Elapsed >= mp.Duration)
                EndGame();
        }

        // Toggle de pausa
        if (hasStartedGameplay && Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        if (!hasStartedGameplay) return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;
    }

    private void EndGame()
    {
        Debug.Log("Fin de la partida.");
        // Ejemplo: volver al menú principal
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}