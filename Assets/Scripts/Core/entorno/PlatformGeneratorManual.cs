using UnityEngine;
using System.Collections.Generic;

public class PlatformGeneratorManual : MonoBehaviour
{
    [System.Serializable]
    class PlatformData
    {
        public GameObject platform;
        public float speed;
        public GameObject enemy;

        public PlatformData(GameObject platform, float speed, GameObject enemy = null)
        {
            this.platform = platform;
            this.speed = speed;
            this.enemy = enemy;
        }
    }

    public GameObject wolfPrefab;
    public GameObject[] platformPrefabs; // 3 prefabs (normal, hielo, arena)
    public float spawnInterval = 1f;
    public float minY = -1.4f, maxY = 7f;
    public float startX = 0f;

    // Spawn de lobos en suelo base
    private float groundWolfTimer = 0f;
    private float groundWolfInterval = 1f; // cada 1 segundo
    private float groundWolfY = -2.97f;
    [Range(0f, 1f)] public float groundWolfSpawnChance = 0.2f;
    private List<GameObject> groundWolves = new List<GameObject>();

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

        // Spawn de lobo en suelo base cada 1 segundo con 40% de probabilidad
        groundWolfTimer += Time.deltaTime;
        if (groundWolfTimer >= groundWolfInterval)
        {
            groundWolfTimer = 0f;
            if (wolfPrefab != null && Random.value < groundWolfSpawnChance)
            {
                Vector3 wolfPos = new Vector3(startX, groundWolfY, -1f);
                GameObject newWolf = Instantiate(wolfPrefab, wolfPos, Quaternion.identity);
                groundWolves.Add(newWolf);
            }
        }

        // Mover plataformas y sus lobos
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            PlatformData data = activePlatforms[i];
            data.platform.transform.position += Vector3.left * data.speed * Time.deltaTime;

            if (data.enemy != null)
                data.enemy.transform.position += Vector3.left * data.speed * Time.deltaTime;

            if (data.platform.transform.position.x <= -51f)
            {
                if (data.enemy != null)
                    Destroy(data.enemy);

                Destroy(data.platform);
                activePlatforms.RemoveAt(i);
            }
        }

        // Mover y eliminar lobos del suelo base
        for (int i = groundWolves.Count - 1; i >= 0; i--)
        {
            GameObject wolf = groundWolves[i];
            wolf.transform.position += Vector3.left * 5f * Time.deltaTime;

            if (wolf.transform.position.x <= -51f)
            {
                Destroy(wolf);
                groundWolves.RemoveAt(i);
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

        string[] tags = { "Ground", "Ice", "Sand" };
        if (prefabIndex >= 0 && prefabIndex < tags.Length)
            newPlat.tag = tags[prefabIndex];
        else
            newPlat.tag = "Ground";

        float[] possibleSpeeds = { 3f, 5f, 8f };
        float selectedSpeed = possibleSpeeds[Random.Range(0, possibleSpeeds.Length)];

        float enemyYOffset = 0.5f;
        var platSR = newPlat.GetComponent<SpriteRenderer>();
        if (platSR != null)
            enemyYOffset = platSR.bounds.size.y / 2f + 0.5f;

        GameObject enemy = null;
        bool spawnEnemy = Random.value < 0.5f;

        if (spawnEnemy && wolfPrefab != null)
        {
            Vector3 enemyPos = new Vector3(spawnPos.x, spawnPos.y + enemyYOffset, -1f);
            enemy = Instantiate(wolfPrefab, enemyPos, Quaternion.identity);
        }

        activePlatforms.Add(new PlatformData(newPlat, selectedSpeed, enemy));
    }
}
