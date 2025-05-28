using UnityEngine;

public class MusicResume : MonoBehaviour
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
            musicPlayer.UnPause();
        else
            Debug.LogWarning("MusicResume: no encontré MusicPlayer");
    }
}