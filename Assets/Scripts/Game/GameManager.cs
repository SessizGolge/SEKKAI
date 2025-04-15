using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Sistemi")]
    [SerializeField] public GameObject[] enemySpawnpoints;
    [SerializeField] public GameObject kagenari;
    [SerializeField] public int wawe = 1; // Başlangıç dalgası
    [SerializeField] public int difficulty = 1; // Zorluk çarpanı
    [SerializeField] public int enemiesPerWave = 3;

    [Header("Player")]
    [SerializeField] public Health playerHealthComponent;

    private List<GameObject> aliveEnemies = new List<GameObject>();
    private bool isSpawning = false;
    public int playerHealth;

    void Start()
    {
        playerHealth = playerHealthComponent.currentHealth;
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);

        if (aliveEnemies.Count == 0 && !isSpawning)
        {
            wawe++;
            difficulty++;
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;

        yield return new WaitForSeconds(2f);

        int totalEnemies = enemiesPerWave * difficulty;

        for (int i = 0; i < totalEnemies; i++)
        {
            int randomIndex = Random.Range(0, enemySpawnpoints.Length);
            Transform spawnPoint = enemySpawnpoints[randomIndex].transform;

            GameObject enemy = Instantiate(kagenari, spawnPoint.position, Quaternion.identity);
            aliveEnemies.Add(enemy);

            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Dalga " + wawe + " başlatıldı. Düşman sayısı: " + totalEnemies);

        isSpawning = false;
    }
}

