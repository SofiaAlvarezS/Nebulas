using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Tooltip("Clip de m�sica de gameplay")]
    public AudioClip gameplayMusic;

    private AudioSource src;

    /// <summary>
    /// Duraci�n total del clip en segundos.
    /// </summary>
    public float Duration => gameplayMusic != null ? gameplayMusic.length : 0f;

    /// <summary>
    /// Tiempo (en segundos) transcurrido de la pista.
    /// </summary>
    public float Elapsed => src != null ? src.time : 0f;

    void Awake()
    {
        // A�adimos AudioSource al GameObject
        src = gameObject.AddComponent<AudioSource>();
        src.clip = gameplayMusic;
        src.loop = false;        // no hacer loop autom�ticamente
        src.playOnAwake = false; // control manual
    }

    /// <summary>
    /// Empieza/reanuda la reproducci�n desde el punto actual.
    /// </summary>
    public void Play()
    {
        if (src != null && gameplayMusic != null)
            src.Play();
    }

    /// <summary>
    /// Pausa la reproducci�n manteniendo la posici�n.
    /// </summary>
    public void Pause()
    {
        if (src != null && src.isPlaying)
            src.Pause();
    }

    /// <summary>
    /// Continua la reproducci�n desde donde se paus�.
    /// </summary>
    public void UnPause()
    {
        if (src != null && !src.isPlaying)
            src.UnPause();
    }

    /// <summary>
    /// Devuelve true si actualmente est� sonando.
    /// </summary>
    public bool IsPlaying => src != null && src.isPlaying;
}
