using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject customizeMenu;
    [SerializeField] private GameObject serverMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject player;

    [Header("UI")]
    [SerializeField] private TMP_Text byMe;
    [SerializeField] private TMP_Text killValue;
    [SerializeField] private TMP_Text soulValue;
    [SerializeField] private TMP_Text nicknameText;

    [Header("Customization")]
    [SerializeField] private Animator playerAnimMenu;
    public SpriteRenderer topVFX;
    public SpriteRenderer torsoVFX;
    public SpriteRenderer bottomVFX;
    [SerializeField] private CostumeDatabase costumeDatabase;

    [HideInInspector] public int kills;
    [HideInInspector] public int Souls;

    private PlayerData playerData;

    public PhotonView PhotonView;

    #region Unity Callbacks
    private IEnumerator Start()
    {
        SoundManager.Instance.Play(SoundManager.Sound.MenuMusic, SoundManager.Instance.musicSource);

        string author = Application.companyName;
        string version = Application.version;
        byMe.text = $"By {author}\nv{version}";

        Cursor.visible = true;

        // Default menü
        OpenMainMenu();

        yield return null;

        // PlayerData sahnede olmalı
        if (PlayerData.Instance == null)
        {
            GameObject pdObj = new GameObject("PlayerData");
            playerData = pdObj.AddComponent<PlayerData>();
            playerData.SaveToPhoton(photonView);
        }
        else
        {
            playerData = PlayerData.Instance;
        }

        RefreshCostume();
    }

    private void Update()
    {
        if (playerData != null)
        {
            nicknameText.text = playerData.Nickname;
        }
        killValue.text = kills.ToString();
        soulValue.text = Souls.ToString();

        if (Input.GetKeyDown(KeyCode.F11))
            Screen.fullScreen = !Screen.fullScreen;
    }
    #endregion

    #region Menu Open/Close
    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        customizeMenu.SetActive(false);
        serverMenu.SetActive(false);
        shopMenu.SetActive(false);

        player.SetActive(true);
        nicknameText.gameObject.SetActive(true);
    }

    public void OpenOptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OpenCustomizeMenu()
    {
        mainMenu.SetActive(false);
        customizeMenu.SetActive(true);
    }

    public void OpenServerMenu()
    {
        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }

    public void OpenShopMenu()
    {
        mainMenu.SetActive(false);
        shopMenu.SetActive(true);
    }

    public void CloseCurrentMenu(GameObject menu)
    {
        menu.SetActive(false);
        OpenMainMenu();
    }
    #endregion

    #region PlayerData & Costumes
    public PlayerData GetPlayerData() => playerData;

    public void RefreshCostume()
    {
        if (playerData != null)
        {
            StartCoroutine(ApplyCostumeWithDelay(playerData.TopIndex, playerData.TorsoIndex, playerData.BottomIndex));
        }
    }

    private IEnumerator ApplyCostumeWithDelay(int topIndex, int torsoIndex, int bottomIndex)
    {
        yield return null; // 1 frame
        if (costumeDatabase.tops.Count > topIndex)
            topVFX.sprite = costumeDatabase.tops[topIndex];

        yield return null; // 1 frame
        if (costumeDatabase.torsos.Count > torsoIndex)
            torsoVFX.sprite = costumeDatabase.torsos[torsoIndex];

        yield return null; // 1 frame
        if (costumeDatabase.bottoms.Count > bottomIndex)
            bottomVFX.sprite = costumeDatabase.bottoms[bottomIndex];
    }
    #endregion
}
