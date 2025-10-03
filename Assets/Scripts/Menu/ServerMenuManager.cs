using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ServerMenuManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private Transform roomListParent;
    [SerializeField] private Transform chatContent; 
    [SerializeField] private GameObject chatMessagePrefab; 
    [SerializeField] private GameObject roomItemPrefab;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject serverMenu;
    [SerializeField] private GameObject player;
    [SerializeField] private TMP_Text nickname;
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button confirmCreateButton;
    [SerializeField] private Button cancelCreateButton;

    [Header("Room UI References")]
    [SerializeField] private GameObject roomUIPanel;
    [SerializeField] private TMP_Text playersListText;
    [SerializeField] private Button startGameButton; 
    [SerializeField] private Button leaveRoomButton;

    [Header("Chat UI References")]
    [SerializeField] private TMP_InputField chatInput;

    private List<GameObject> roomItems = new List<GameObject>();
    private PlayerData playerData;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        playerData = FindObjectOfType<MenuManager>().GetPlayerData();
        nickname.text = playerData.Nickname;

        createRoomPanel.SetActive(false);
        roomUIPanel.SetActive(false);
        roomNameInput.characterLimit = 20;

        createRoomButton.onClick.AddListener(() => { OpenCreateRoomPanel(); PlayChooseSFX(); });
        confirmCreateButton.onClick.AddListener(() => { CreateRoom(); PlayChooseSFX(); });
        cancelCreateButton.onClick.AddListener(() => { CloseCreateRoomPanel(); PlaySelectSFX(); });

        chatInput.onEndEdit.AddListener(OnChatSubmitted);
    }

    private void OnDestroy()
    {
        chatInput.onEndEdit.RemoveListener(OnChatSubmitted);
    }

    #region Chat
    private void OnChatSubmitted(string text)
    {
        if (!string.IsNullOrWhiteSpace(text) && Input.GetKeyDown(KeyCode.Return))
        {
            string message = text.Trim();
            chatInput.text = string.Empty;
            chatInput.ActivateInputField();
            pv.RPC("RPC_ReceiveChatMessage", RpcTarget.All, playerData.Nickname, message);
        }
    }

    [PunRPC]
    private void RPC_ReceiveChatMessage(string sender, string message)
    {
        GameObject go = Instantiate(chatMessagePrefab, chatContent);
        TMP_Text txt = go.GetComponentInChildren<TMP_Text>();
        txt.text = $"<{sender}> {message}";
        SoundManager.Instance.Play(SoundManager.Sound.NormalMessageSFX, SoundManager.Instance.sfxSource);

        int charsPerLine = 30;
        int lineCount = Mathf.CeilToInt((float)txt.text.Length / charsPerLine);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, lineCount * 60);

        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());
        chatContent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0f;
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Connected to Master Server, joined Lobby.");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var item in roomItems)
            if (item != null) Destroy(item);
        roomItems.Clear();

        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList) continue;

            GameObject go = Instantiate(roomItemPrefab, roomListParent);
            roomItems.Add(go);

            TMP_Text roomNameTxt = go.transform.Find("RoomName").GetComponent<TMP_Text>();
            TMP_Text playerCountTxt = go.transform.Find("PlayerCount").GetComponent<TMP_Text>();
            Button joinBtn = go.GetComponent<Button>();

            roomNameTxt.text = info.Name;
            playerCountTxt.text = info.PlayerCount + " / " + info.MaxPlayers;

            joinBtn.onClick.AddListener(() => { JoinRoom(info.Name); PlayChooseSFX(); });
        }
    }

    public override void OnJoinedRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        serverMenu.SetActive(false);
        player.SetActive(false);
        nickname.gameObject.SetActive(false);
        roomUIPanel.SetActive(true);
        UpdatePlayersList();

        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        leaveRoomButton.onClick.RemoveAllListeners();
        leaveRoomButton.onClick.AddListener(() => { LeaveRoom(); PlaySelectSFX(); });

        startGameButton.onClick.RemoveAllListeners();
        if (PhotonNetwork.IsMasterClient)
            startGameButton.onClick.AddListener(() => { StartGame(); PlayChooseSFX(); });
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient)
            MessageManager.Instance.ShowMessage("You are now the room leader!", Color.yellow, MessageManager.MessageType.Normal);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        MessageManager.Instance.ShowMessage("Failed to join room!", Color.red, MessageManager.MessageType.Error);
        PlaySelectSFX();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MessageManager.Instance.ShowMessage("Failed to create room!", Color.red, MessageManager.MessageType.Error);
        PlaySelectSFX();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) => UpdatePlayersList();
    public override void OnPlayerLeftRoom(Player otherPlayer) => UpdatePlayersList();
    public override void OnLeftRoom()
    {
        roomUIPanel.SetActive(false);
        serverMenu.SetActive(true);
        player.SetActive(true);
        nickname.gameObject.SetActive(true);
    }
    #endregion

    #region UI Methods
    public void OpenCreateRoomPanel()
    {
        createRoomPanel.SetActive(true);
        serverMenu.SetActive(false);
        player.SetActive(false);
        nickname.gameObject.SetActive(false);
    }

    public void CloseCreateRoomPanel()
    {
        createRoomPanel.SetActive(false);
        serverMenu.SetActive(true);
        player.SetActive(true);
        nickname.gameObject.SetActive(true);
        roomNameInput.text = "";
    }

    public void CreateRoom()
    {
        string roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            MessageManager.Instance.ShowMessage("Room name can't be empty!", Color.red, MessageManager.MessageType.Error);
            PlaySelectSFX();
            return;
        }

        RoomOptions options = new RoomOptions() { MaxPlayers = 6 };
        PhotonNetwork.CreateRoom(roomName, options);
        CloseCreateRoomPanel();
    }

    private void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        MessageManager.Instance.ShowMessage("You joined the room!", Color.green, MessageManager.MessageType.Normal);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MessageManager.Instance.ShowMessage("You left the room!", Color.Lerp(Color.red, Color.yellow, 0.5f), MessageManager.MessageType.Normal);
    }

    private void UpdatePlayersList()
    {
        string listText = "";
        foreach (Player p in PhotonNetwork.PlayerList)
            listText += p.NickName + "\n";
        playersListText.text = listText;
    }
    #endregion

    #region Game Start Methods
    private void StartGame()
    {
        startGameButton.interactable = false;
        pv.RPC("RPC_StartCountdown", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_StartCountdown() => StartCoroutine(StartGameCountdown());

    private IEnumerator StartGameCountdown()
    {
        MessageManager.Instance.ShowMessage("Starting the game...", Color.white, MessageManager.MessageType.Normal);
        yield return new WaitForSeconds(1f);

        MessageManager.Instance.ShowMessage("3", Color.red, MessageManager.MessageType.Normal);
        yield return new WaitForSeconds(1f);

        MessageManager.Instance.ShowMessage("2", Color.Lerp(Color.red, Color.yellow, 0.5f), MessageManager.MessageType.Normal);
        yield return new WaitForSeconds(1f);

        MessageManager.Instance.ShowMessage("1", Color.yellow, MessageManager.MessageType.Normal);
        yield return new WaitForSeconds(1f);

        MessageManager.Instance.ShowMessage("GO!", Color.green, MessageManager.MessageType.Normal);
        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(1);
    }
    #endregion

    #region Sound Methods
    private void PlayChooseSFX() => SoundManager.Instance.Play(SoundManager.Sound.ChooseSFX, SoundManager.Instance.sfxSource);
    private void PlaySelectSFX() => SoundManager.Instance.Play(SoundManager.Sound.SelectSFX, SoundManager.Instance.sfxSource);
    #endregion
}
