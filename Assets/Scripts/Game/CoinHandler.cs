using UnityEngine;

public class CoinHandler : MonoBehaviour
{
    [SerializeField] private int minCoin = 0;
    [SerializeField] private int maxCoin = 20;
    [SerializeField] private Health healthComponent;
    [SerializeField] private PlayerController player;
    [SerializeField] public AudioSource sfxSource;
    [SerializeField] public AudioClip coinSFX;

    private bool hasGivenReward = false;

    void Start()
    {
        if (healthComponent == null)
            healthComponent = GetComponent<Health>();
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (healthComponent.IsDead && !hasGivenReward)
        {
            GiveCoinReward();
            hasGivenReward = true;
        }
    }

    void GiveCoinReward()
    {
        int coinAmount = Random.Range(minCoin, maxCoin + 1);
        PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") + coinAmount); // var olan değere ekle
        sfxSource.PlayOneShot(coinSFX);
    }
}
