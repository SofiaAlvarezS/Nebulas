using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Tooltip("Clip de música de gameplay")]
    public AudioClip gameplayMusic;

    private AudioSource src;

    /// <summary>
    /// Duración total del clip en segundos.
    /// </summary>
    public float Duration => gameplayMusic != null ? gameplayMusic.length : 0f;

    /// <summary>
    /// Tiempo (en segundos) transcurrido de la pista.
    /// </summary>
    public float Elapsed => src != null ? src.time : 0f;

    void Awake()
    {
        // Añadimos AudioSource al GameObject
        src = gameObject.AddComponent<AudioSource>();
        src.clip = gameplayMusic;
        src.loop = false;        // no hacer loop automáticamente
        src.playOnAwake = false; // control manual
    }

    /// <summary>
    /// Empieza/reanuda la reproducción desde el punto actual.
    /// </summary>
    public void Play()
    {
        if (src != null && gameplayMusic != null)
            src.Play();
    }

    /// <summary>
    /// Pausa la reproducción manteniendo la posición.
    /// </summary>
    public void Pause()
    {
        if (src != null && src.isPlaying)
            src.Pause();
    }

    /// <summary>
    /// Continua la reproducción desde donde se pausó.
    /// </summary>
    public void UnPause()
    {
        if (src != null && !src.isPlaying)
            src.UnPause();
    }

    /// <summary>
    /// Devuelve true si actualmente está sonando.
    /// </summary>
    public bool IsPlaying => src != null && src.isPlaying;
}
