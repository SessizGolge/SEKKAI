using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Cinemachine;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    IEnumerator Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab eksik!");
        }

        SetLocalCostumeToPhoton();

        GameObject playerInstance;

        if (spawnPoints.Count == 0)
        {
            playerInstance = SpawnRandom();
        }
        else
        {
            int index = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Count;
            Transform spawnPoint = spawnPoints[index];
            playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }

        yield return null;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject pGO = (p == PhotonNetwork.LocalPlayer) ? playerInstance : FindPlayerGOByPhoton(p);
            if (pGO != null)
                ApplyPlayerCostume(p, pGO);
        }

        if (virtualCamera != null && playerInstance != null)
        {
            virtualCamera.Follow = playerInstance.transform;
            virtualCamera.LookAt = playerInstance.transform;
        }
    }

    private void SetLocalCostumeToPhoton()
    {
        int top = PlayerPrefs.GetInt("TopIndex", 0);
        int torso = PlayerPrefs.GetInt("TorsoIndex", 0);
        int bottom = PlayerPrefs.GetInt("BottomIndex", 0);

        ExitGames.Client.Photon.Hashtable costumeProps = new ExitGames.Client.Photon.Hashtable();
        costumeProps["TopIndex"] = top;
        costumeProps["TorsoIndex"] = torso;
        costumeProps["BottomIndex"] = bottom;

        PhotonNetwork.LocalPlayer.SetCustomProperties(costumeProps);
    }

    public void ApplyPlayerCostume(Player player, GameObject playerGO)
    {
        var props = player.CustomProperties;

        int top = props.ContainsKey("TopIndex") ? (int)props["TopIndex"] : 0;
        int torso = props.ContainsKey("TorsoIndex") ? (int)props["TorsoIndex"] : 0;
        int bottom = props.ContainsKey("BottomIndex") ? (int)props["BottomIndex"] : 0;

        var cm = playerGO.GetComponent<PlayerCostumeManager>();
        StartCoroutine(ApplyCostumeNextFrame(cm, top, torso, bottom));
    }

    private IEnumerator ApplyCostumeNextFrame(PlayerCostumeManager cm, int top, int torso, int bottom)
    {
        yield return null; // 1 frame bekle
        cm.SetCostume(top, torso, bottom);
    }

    private GameObject SpawnRandom()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0f);
        return PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);
    }

    private GameObject FindPlayerGOByPhoton(Player player)
    {
        foreach (var go in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView pv = go.GetComponent<PhotonView>();
            if (pv != null && pv.Owner == player)
                return go;
        }
        return null;
    }
}
