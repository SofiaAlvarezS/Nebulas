using UnityEngine;
using System.Collections.Generic;

public class PlatformGeneratorManual : MonoBehaviour
{
    [System.Serializable]
    class PlatformData
    {
        public GameObject platform;
        public float speed;

        public PlatformData(GameObject platform, float speed)
        {
            this.platform = platform;
            this.speed = speed;
        }
    }

    public GameObject[] platformPrefabs; // Asigna aquí los 3 prefabs
    public float spawnInterval = 2f;
    public float minY = -1.4f, maxY = 7f;
    public float startX = 0f;

    private float timer;
    private List<PlatformData> activePlatforms = new List<PlatformData>();

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPlatform();
            timer = 0f;
        }

        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            PlatformData data = activePlatforms[i];
            data.platform.transform.position += Vector3.left * data.speed * Time.deltaTime;

            if (data.platform.transform.position.x <= -53f)
            {
                Destroy(data.platform);
                activePlatforms.RemoveAt(i);
            }
        }
    }

    void SpawnPlatform()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(startX, randomY, -1f);

        int prefabIndex = Random.Range(0, platformPrefabs.Length);
        GameObject prefabToSpawn = platformPrefabs[prefabIndex];
        GameObject newPlat = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        float[] possibleSpeeds = { 3f, 5f, 8f };
        float selectedSpeed = possibleSpeeds[Random.Range(0, possibleSpeeds.Length)];

        activePlatforms.Add(new PlatformData(newPlat, selectedSpeed));

        var sr = newPlat.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            var b = sr.bounds;
            GameObject.FindWithTag("Player")
                      ?.GetComponent<PlayerController2D>()
                      ?.AddGroundRect(new Rect(b.min.x, b.min.y, b.size.x, b.size.y));
        }
    }
}
