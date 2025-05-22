using UnityEngine;

[System.Serializable]
public class LayerData
{
    public Transform layer;      // tu GameObject (Cielo, Montañas…)
    public float speed = 1f;     // velocidad de scroll (unidades/segundo)
    [HideInInspector] public float width;
    [HideInInspector] public Vector3 startPos;
}

public class UVscroller : MonoBehaviour
{
    public LayerData[] layers;

    void Start()
    {
        // Guarda posición inicial y ancho de cada capa
        foreach (var L in layers)
        {
            L.startPos = L.layer.position;
            var sr = L.layer.GetComponent<SpriteRenderer>();
            if (sr == null)
                Debug.LogError($"[{name}] El objeto {L.layer.name} no tiene SpriteRenderer.");
            else
                L.width = sr.bounds.size.x;
        }
    }

    void Update()
    {
        foreach (var L in layers)
        {
            // Mover
            L.layer.position += Vector3.left * L.speed * Time.deltaTime;

            // Loop: si avanzó más que su ancho, vuelve al inicio
            if (L.startPos.x - L.layer.position.x >= L.width)
                L.layer.position = L.startPos;
        }
    }
}
