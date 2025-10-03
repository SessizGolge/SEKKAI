using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Photon.Pun;

public class ShopMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] costumeButtons;
    public Button buyButton;
    public TMP_Text costumeNameText;

    [Header("Player Parts")]
    public SpriteRenderer topVFX;
    public SpriteRenderer torsoVFX;
    public SpriteRenderer bottomVFX;
    public CostumeDatabase database;
    public MenuManager menuManager;

    [Header("Currency")]
    public int playerSouls;
    public int cost;

    private int selectedCostumeIndex = 0;
    private PlayerData playerData;
    [SerializeField] Sprite unequipSprite;
    [SerializeField] Sprite equipSprite;
    [SerializeField] Sprite buySprite;
    [SerializeField] public TMP_Text soulValue, costText;
    public GameObject costParent;

    void Start()
    {
        playerData = menuManager.GetPlayerData(); // MenuManager üzerinden alıyoruz

        for (int i = 0; i < costumeButtons.Length; i++)
        {
            int index = i;
            costumeButtons[i].onClick.AddListener(() => { SelectCostume(index); PlaySelectSFX(); });
        }

        buyButton.onClick.AddListener(() => { BuyOrEquipCostume(); PlayPurchaseSFX(); });

        SelectCostume(0);
    }

    void Update()
    {
        if (playerData != null && soulValue != null)
        {
            soulValue.text = playerData.OwnedCostumes.Count.ToString();
        }

        if (menuManager != null)
            playerSouls = menuManager.Souls; // MenuManager'dan al
    }

    void SelectCostume(int index)
    {
        costumeNameText.gameObject.SetActive(true);
        selectedCostumeIndex = index;

        cost = database.costumeCost[index];
        string name = database.costumeNames[index];

        if (playerData.OwnedCostumes.Contains(index))
        {
            costParent.SetActive(true);
            costumeNameText.text = name;
            costParent.GetComponent<Image>().enabled = false;
            costText.text = "Owned";
            costText.color = Color.green;
            UpdateBuyButton();
        }
        else
        {
            costParent.SetActive(true);
            costParent.GetComponent<Image>().enabled = true;
            costumeNameText.text = name;
            costText.text = cost.ToString();
            costText.color = playerSouls >= cost ? Color.Lerp(Color.red, Color.yellow, 0.5f) : Color.red;
            UpdateBuyButton();
        }

        StartCoroutine(ApplyCostumeWithDelay(index));
    }

    System.Collections.IEnumerator ApplyCostumeWithDelay(int index)
    {
        yield return null; // 1 frame gecikme
        topVFX.sprite = database.tops[index];
        yield return null; // 1 frame daha
        torsoVFX.sprite = database.torsos[index];
        yield return null;
        bottomVFX.sprite = database.bottoms[index];
    }

    void UpdateBuyButton()
    {
        if (playerData.OwnedCostumes.Contains(selectedCostumeIndex))
        {
            buyButton.gameObject.SetActive(false);
            buyButton.image.sprite = unequipSprite;
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            buyButton.image.sprite = buySprite;
        }
    }

    void BuyOrEquipCostume()
    {
        if (!playerData.OwnedCostumes.Contains(selectedCostumeIndex))
        {
            if (playerSouls >= cost)
            {
                playerSouls -= cost;
                menuManager.Souls = playerSouls;

                playerData.OwnedCostumes.Add(selectedCostumeIndex);
                playerData.SaveToPhoton(menuManager.photonView);

                StartCoroutine(ApplyCostumeWithDelay(selectedCostumeIndex));

                MessageManager.Instance.ShowMessage("You bought " + database.costumeNames[selectedCostumeIndex] + "!", Color.green, MessageManager.MessageType.Normal);

                costParent.GetComponent<Image>().enabled = false;
                costText.text = "Owned";
                costText.color = Color.green;
                UpdateBuyButton();

                SoundManager.Instance.Play(SoundManager.Sound.PurchaseSFX, SoundManager.Instance.sfxSource);
            }
            else
            {
                MessageManager.Instance.ShowMessage("Not enough Soul Essence!", Color.red, MessageManager.MessageType.Error);
                SoundManager.Instance.Play(SoundManager.Sound.ChooseSFX, SoundManager.Instance.sfxSource);
            }
        }
    }

    void PlaySelectSFX() => SoundManager.Instance.Play(SoundManager.Sound.SelectSFX, SoundManager.Instance.sfxSource);
    void PlayPurchaseSFX() => SoundManager.Instance.Play(SoundManager.Sound.PurchaseSFX, SoundManager.Instance.sfxSource);
}
