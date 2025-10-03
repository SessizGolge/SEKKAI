using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerData : MonoBehaviourPun
{
    public static PlayerData Instance;

    [Header("Player Info")]
    public string Nickname;
    public int TopIndex;
    public int TorsoIndex;
    public int BottomIndex;
    public List<int> OwnedCostumes = new List<int>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Varsayılan değerler
        if (string.IsNullOrEmpty(Nickname))
            Nickname = PhotonNetwork.NickName;

        if (OwnedCostumes == null || OwnedCostumes.Count == 0)
            OwnedCostumes = new List<int> { 0, 1 }; // Default owned costumes

        TopIndex = 0;
        TorsoIndex = 0;
        BottomIndex = 0;
    }

    // Photon’a kaydetmek için
    public void SaveToPhoton(PhotonView view)
    {
        object[] data = new object[]
        {
            Nickname,
            TopIndex,
            TorsoIndex,
            BottomIndex,
            OwnedCostumes.ToArray()
        };
        view.RPC("RPC_UpdatePlayerData", RpcTarget.AllBuffered, data);
    }

    [PunRPC]
    private void RPC_UpdatePlayerData(object[] data)
    {
        Nickname = (string)data[0];
        TopIndex = (int)data[1];
        TorsoIndex = (int)data[2];
        BottomIndex = (int)data[3];
        OwnedCostumes = new List<int>((int[])data[4]);
    }
}
