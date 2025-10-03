using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance; // Singleton

    [SerializeField] GameObject messagePrefab;
    [SerializeField] Transform messageParent;

    private GameObject activeMessage; // Şu anda sahnede olan mesaj

    public enum MessageType
    {
        Normal,
        Error
    }

    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowMessage(string text, Color color, MessageType type = MessageType.Normal, float duration = 3f)
    {
        // Eğer sahnede bir mesaj varsa sil
        if (activeMessage != null)
        {
            Destroy(activeMessage);
            activeMessage = null;
        }

        // Yeni mesaj instantiate et
        activeMessage = Instantiate(messagePrefab, messageParent);
        TMP_Text tmp = activeMessage.GetComponent<TMP_Text>();
        tmp.text = text;
        tmp.color = color;

        // Ses çal
        switch (type)
        {
            case MessageType.Normal:
                SoundManager.Instance.Play(SoundManager.Sound.NormalMessageSFX, SoundManager.Instance.sfxSource);
                break;
            case MessageType.Error:
                SoundManager.Instance.Play(SoundManager.Sound.ErrorMessageSFX, SoundManager.Instance.sfxSource);
                break;
        }

        // Belirtilen süre sonra sil
        Destroy(activeMessage, duration);
    }
}
