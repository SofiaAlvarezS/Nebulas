using UnityEngine;
using UnityEngine.UI;

public class Corazones : MonoBehaviour
{
    [Header("Sprites de corazones")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Referencias a las imágenes en pantalla")]
    public Image[] hearts;

    public void UpdateHearts(float currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}