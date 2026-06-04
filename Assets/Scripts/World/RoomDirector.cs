using UnityEngine;
using System.Collections.Generic;

public class RoomDirector : MonoBehaviour
{
    [Header("Enemys")]
    [SerializeField] private List<GameObject> enemyPrefabs; 
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float enemyInterval = 10f;
    [SerializeField] private int maxEnemiesInRoom = 2;
    
    private float enemyTimer;
    private List<GameObject> activeEnemies = new List<GameObject>(); 
    private Transform lastSpawnPoint;

    [Header("Sanity Items")]
    [SerializeField] private GameObject sanityItemPrefab;
    [SerializeField] private List<Transform> sanitySpawnPoints;
    [SerializeField] private float sanitySpawnInterval = 20f;
    
    private float sanityTimer;
    private PlayerSanity playerSanity;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerSanity = player.GetComponent<PlayerSanity>();
        }
       
        int progresoGlobal = WorldSceneManager.Instance.worldState.hab1Visits;
        enemyInterval = Mathf.Max(3f, enemyInterval - progresoGlobal); 
    }

    void Update()
    {
        // ENEMIGOS
        activeEnemies.RemoveAll(enemy => enemy == null);
        enemyTimer += Time.deltaTime;

        if (enemyTimer >= enemyInterval && enemyPrefabs.Count > 0)
        {
            if (activeEnemies.Count < maxEnemiesInRoom)
            {
                GameObject selectedEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                SpawnEnemy(selectedEnemy, spawnPoints);
            }
            enemyTimer = 0;
        }

        // CORDURA
        float currentSanity = (playerSanity != null) ? playerSanity.CurrentSanity : 100f;
        
        float sanityMultiplier = (currentSanity < 50f) ? 0.5f : 1.0f; 
        float actualSanityInterval = sanitySpawnInterval * sanityMultiplier;

        sanityTimer += Time.deltaTime;

        if (sanityTimer >= actualSanityInterval && sanityItemPrefab != null && sanitySpawnPoints.Count > 0)
        {
            SpawnSanityItem(sanityItemPrefab, sanitySpawnPoints);
            sanityTimer = 0;
        }
    }

    private void SpawnEnemy(GameObject prefab, List<Transform> points)
    {
        if (points.Count == 0 || prefab == null) return;

        Transform selectedSpawn = null;

        if (points.Count > 1)
        {
            List<Transform> availablePoints = new List<Transform>(points);
            
            if (lastSpawnPoint != null && availablePoints.Contains(lastSpawnPoint))
            {
                availablePoints.Remove(lastSpawnPoint);
            }
            
            selectedSpawn = availablePoints[Random.Range(0, availablePoints.Count)];
        }
        else
        {
            selectedSpawn = points[0];
        }

        lastSpawnPoint = selectedSpawn; 

        GameObject newEnemy = Instantiate(prefab, selectedSpawn.position, selectedSpawn.rotation);
        activeEnemies.Add(newEnemy); // Lo añadimos a la lista de control
    }

    private void SpawnSanityItem(GameObject prefab, List<Transform> points)
    {
        if (points.Count == 0 || prefab == null) return;

        Transform selectedSpawn = points[Random.Range(0, points.Count)];
        Instantiate(prefab, selectedSpawn.position, selectedSpawn.rotation);
    }
}