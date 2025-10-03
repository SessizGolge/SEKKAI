using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CustomizeMenuManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button topLeftButton, topRightButton;
    public Button torsoLeftButton, torsoRightButton;
    public Button bottomLeftButton, bottomRightButton;

    [Header("Player Parts")]
    public SpriteRenderer topVFX, torsoVFX, bottomVFX;
    public CostumeDatabase costumeDatabase;

    private int selectedTopIndex;
    private int selectedTorsoIndex;
    private int selectedBottomIndex;

    private PlayerData playerData;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        playerData = FindObjectOfType<MenuManager>().GetPlayerData();

        LoadSelection();

        topLeftButton.onClick.AddListener(() => { ChangeTop(-1); });
        topRightButton.onClick.AddListener(() => { ChangeTop(1); });
        torsoLeftButton.onClick.AddListener(() => { ChangeTorso(-1); });
        torsoRightButton.onClick.AddListener(() => { ChangeTorso(1); });
        bottomLeftButton.onClick.AddListener(() => { ChangeBottom(-1); });
        bottomRightButton.onClick.AddListener(() => { ChangeBottom(1); });
    }

    public void LoadSelection()
    {
        selectedTopIndex = playerData.TopIndex;
        selectedTorsoIndex = playerData.TorsoIndex;
        selectedBottomIndex = playerData.BottomIndex;

        StartCoroutine(ApplyCostumeWithDelay(selectedTopIndex, selectedTorsoIndex, selectedBottomIndex));
    }

    void ChangeTop(int delta)
    {
        selectedTopIndex = (selectedTopIndex + delta + costumeDatabase.tops.Count) % costumeDatabase.tops.Count;
        playerData.TopIndex = selectedTopIndex;
        playerData.SaveToPhoton(photonView);
        StartCoroutine(ApplyCostumeWithDelay(selectedTopIndex, selectedTorsoIndex, selectedBottomIndex));
    }

    void ChangeTorso(int delta)
    {
        selectedTorsoIndex = (selectedTorsoIndex + delta + costumeDatabase.torsos.Count) % costumeDatabase.torsos.Count;
        playerData.TorsoIndex = selectedTorsoIndex;
        playerData.SaveToPhoton(photonView);
        StartCoroutine(ApplyCostumeWithDelay(selectedTopIndex, selectedTorsoIndex, selectedBottomIndex));
    }

    void ChangeBottom(int delta)
    {
        selectedBottomIndex = (selectedBottomIndex + delta + costumeDatabase.bottoms.Count) % costumeDatabase.bottoms.Count;
        playerData.BottomIndex = selectedBottomIndex;
        playerData.SaveToPhoton(photonView);
        StartCoroutine(ApplyCostumeWithDelay(selectedTopIndex, selectedTorsoIndex, selectedBottomIndex));
    }

    IEnumerator ApplyCostumeWithDelay(int topIndex, int torsoIndex, int bottomIndex)
    {
        yield return null;
        topVFX.sprite = costumeDatabase.tops[topIndex];
        yield return null;
        torsoVFX.sprite = costumeDatabase.torsos[torsoIndex];
        yield return null;
        bottomVFX.sprite = costumeDatabase.bottoms[bottomIndex];
    }
}
