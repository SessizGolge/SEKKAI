using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text enemyCountText;
    [SerializeField] private TMP_Text waveInfoText;

    [Header("Spawn Sistemi")]
    [SerializeField] public GameObject[] enemySpawnpoints;
    [SerializeField] public GameObject kagenari;
    [SerializeField] public int wawe = 1;
    [SerializeField] public int difficulty = 1;
    [SerializeField] public int enemiesPerWave = 3;

    [Header("Player")]
    [SerializeField] public Health playerHealthComponent;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform playerTransform;
    [SerializeField] public AudioSource sfxSource;
    [SerializeField] public AudioClip waveStartedSound;
    [SerializeField] public Animator animPlayer;

    private List<GameObject> aliveEnemies = new List<GameObject>();
    private bool isSpawning = false;

    public int totalEnemiesKilled = 0;
    public int playerHealth;
    public float wavePrepareTime = 0.1f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Screen.SetResolution(1600, 1200, true);

        PlayerPrefs.SetInt("KilledEnemies", 0);
        PlayerPrefs.SetInt("WavesCompleted", 1);

        playerHealth = playerHealthComponent.currentHealth;
        StartCoroutine(SpawnWave());
    }

    void Update()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
        UpdateEnemyUI();

        if (aliveEnemies.Count == 0 && !isSpawning)
        {
            wawe++;
            difficulty++;
            PlayerPrefs.SetInt("WavesCompleted", wawe);

            int wavePR = PlayerPrefs.GetInt("PR_WavesCompleted", 1);
            if (wawe > wavePR)
            {
                PlayerPrefs.SetInt("PR_WavesCompleted", wawe);
            }

            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        yield return new WaitForSeconds(1f);
        // Oyuncuyu sabitle ve spawn noktasına taşı
        if (playerTransform != null && playerSpawn != null)
        {
            playerTransform.position = playerSpawn.position;

            var movement = playerTransform.GetComponent<PlayerController>();
            if (movement != null)
            {
                movement.shouldMove = false;
                animPlayer.SetBool("AkaiStand", true);
                animPlayer.SetBool("AkaiWalk", false);
            }
        }

        ShowWaveInfo("Preparing Next Wave...");
        yield return new WaitForSeconds(2f);

        int totalEnemies = enemiesPerWave * difficulty;
        int extraDamage = (wawe / 3) + 1;

        for (int i = 0; i < totalEnemies; i++)
        {
            int randomIndex = Random.Range(0, enemySpawnpoints.Length);
            Transform spawnPoint = enemySpawnpoints[randomIndex].transform;

            GameObject enemy = Instantiate(kagenari, spawnPoint.position, Quaternion.identity);

            // Düşman hasarını artır
            EnemyController enemyDamage = enemy.GetComponent<EnemyController>();
            if (enemyDamage != null)
            {
                enemyDamage.damage += extraDamage;
            }

            aliveEnemies.Add(enemy);
            yield return new WaitForSeconds(wavePrepareTime);
        }

        // Oyuncuya kontrolü geri ver
        if (playerTransform != null)
        {
            var movement = playerTransform.GetComponent<PlayerController>();
            if (movement != null)
                movement.shouldMove = true;
        }


        ShowWaveInfo("The Next Wave Started!");
        sfxSource.PlayOneShot(waveStartedSound);
        yield return new WaitForSeconds(2f);
        ShowWaveInfo(""); // Mesajı temizle

        isSpawning = false;
    }

    public void EnemyKilled()
    {
        totalEnemiesKilled++;
        PlayerPrefs.SetInt("KilledEnemies", totalEnemiesKilled);

        int killPR = PlayerPrefs.GetInt("PR_KilledEnemies", 0);
        if (totalEnemiesKilled > killPR)
        {
            PlayerPrefs.SetInt("PR_KilledEnemies", totalEnemiesKilled);
        }
    }

    private void UpdateEnemyUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = aliveEnemies.Count.ToString();
        }
    }

    private void ShowWaveInfo(string message)
    {
        if (waveInfoText != null)
        {
            waveInfoText.text = message;
            waveInfoText.alpha = 1f;
        }
    }
}
