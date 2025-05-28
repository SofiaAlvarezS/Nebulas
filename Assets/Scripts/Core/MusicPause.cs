using UnityEngine;

public class MusicPause : MonoBehaviour
{
    [Tooltip("Referencia al MusicPlayer en escena")]
    public MusicPlayer musicPlayer;

    void Awake()
    {
        if (musicPlayer == null)
            musicPlayer = Object.FindFirstObjectByType<MusicPlayer>();
    }

    void OnMouseDown()
    {
        if (musicPlayer != null)
            musicPlayer.Pause();
        else
            Debug.LogWarning("MusicPause: no encontré MusicPlayer");
    }
}