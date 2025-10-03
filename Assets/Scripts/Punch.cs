using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;

public class LayeredPunchController : MonoBehaviour
{
    [Header("Animator")]
    public Animator anim;

    [Header("Sprite Renderers")]
    public SpriteRenderer topRenderer;
    public SpriteRenderer torsoRenderer;
    public SpriteRenderer bottomRenderer;

    [Header("Kostüm Sprites")]
    public Sprite noneTop, noneTorso, noneBottom;
    public Sprite akaiTop, akaiTorso, akaiBottom;
    public Sprite shiroTop, shiroTorso, shiroBottom;
    public Sprite mizumiTop, mizumiTorso, mizumiBottom;
    public Sprite josephTop, josephTorso, josephBottom;

    [Header("Animation Clips")]
    public AnimationClip noneTopPunch, noneTorsoPunch, noneBottomPunch;
    public AnimationClip akaiTopPunch, akaiTorsoPunch, akaiBottomPunch;
    public AnimationClip shiroTopPunch, shiroTorsoPunch, shiroBottomPunch;
    public AnimationClip mizumiTopPunch, mizumiTorsoPunch, mizumiBottomPunch;
    public AnimationClip josephTopPunch, josephTorsoPunch, josephBottomPunch;

    private PlayableGraph graph;
    private AnimationLayerMixerPlayable mixer;

    private Dictionary<int, AnimationClip[]> costumeClips = new Dictionary<int, AnimationClip[]>();
    private Dictionary<AnimationClip, AnimationClipPlayable> clipPlayables = new Dictionary<AnimationClip, AnimationClipPlayable>();
    
    private bool isPunchPlaying = false;
    private float topDuration, torsoDuration, bottomDuration;
    private float topTimer, torsoTimer, bottomTimer;

    private int currentCostumeIndex = 0;

    void Start()
    {
        graph = PlayableGraph.Create("LayeredPunchGraph");
        var output = AnimationPlayableOutput.Create(graph, "Anim", anim);

        mixer = AnimationLayerMixerPlayable.Create(graph, 3);
        output.SetSourcePlayable(mixer);

        // Tüm clip'leri önceden cache'le
        InitializeCostumeClips();
        PrecreateAllPlayables();

        // Başlangıç kostümünü ayarla
        SetCostume(0);

        graph.Play();
    }

    void InitializeCostumeClips()
    {
        costumeClips.Add(0, new AnimationClip[3] { noneTopPunch, noneTorsoPunch, noneBottomPunch });
        costumeClips.Add(1, new AnimationClip[3] { akaiTopPunch, akaiTorsoPunch, akaiBottomPunch });
        costumeClips.Add(2, new AnimationClip[3] { shiroTopPunch, shiroTorsoPunch, shiroBottomPunch });
        costumeClips.Add(3, new AnimationClip[3] { mizumiTopPunch, mizumiTorsoPunch, mizumiBottomPunch });
        costumeClips.Add(4, new AnimationClip[3] { josephTopPunch, josephTorsoPunch, josephBottomPunch });
    }

    void PrecreateAllPlayables()
    {
        // Tüm kostümlerin tüm clip'leri için playable oluştur
        foreach (var costume in costumeClips.Values)
        {
            foreach (var clip in costume)
            {
                if (clip != null && !clipPlayables.ContainsKey(clip))
                {
                    var playable = AnimationClipPlayable.Create(graph, clip);
                    clipPlayables.Add(clip, playable);
                    
                    // Playable'ı pause ederek başlat
                    playable.Pause();
                }
            }
        }

        // Başlangıçta mixer input'larını bağla
        SetPlayablesForCostume(0);
    }

    void SetPlayablesForCostume(int costumeIndex)
    {
        if (!costumeClips.ContainsKey(costumeIndex)) return;

        var clips = costumeClips[costumeIndex];
        
        // Mixer input'larını değiştir
        mixer.DisconnectInput(0);
        mixer.DisconnectInput(1);
        mixer.DisconnectInput(2);

        mixer.ConnectInput(0, clipPlayables[clips[0]], 0);
        mixer.ConnectInput(1, clipPlayables[clips[1]], 0);
        mixer.ConnectInput(2, clipPlayables[clips[2]], 0);

        topDuration = clips[0].length;
        torsoDuration = clips[1].length;
        bottomDuration = clips[2].length;

        mixer.SetInputWeight(0, 0f);
        mixer.SetInputWeight(1, 0f);
        mixer.SetInputWeight(2, 0f);
    }

    void Update()
    {
        // Kostüm değişimi
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetCostume(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetCostume(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetCostume(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetCostume(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetCostume(4);

        // Punch tetikle
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlayPunch();
        }

        // Animasyon sürelerini takip et
        if (isPunchPlaying)
        {
            topTimer += Time.deltaTime;
            torsoTimer += Time.deltaTime;
            bottomTimer += Time.deltaTime;

            if (topTimer >= topDuration && torsoTimer >= torsoDuration && bottomTimer >= bottomDuration)
            {
                isPunchPlaying = false;
                mixer.SetInputWeight(0, 0f);
                mixer.SetInputWeight(1, 0f);
                mixer.SetInputWeight(2, 0f);
            }
        }
    }

    void SetCostume(int index)
    {
        // ÖNCE ANİMASYONU DURDUR
        if (isPunchPlaying)
        {
            mixer.SetInputWeight(0, 0f);
            mixer.SetInputWeight(1, 0f);
            mixer.SetInputWeight(2, 0f);
            isPunchPlaying = false;
        }

        currentCostumeIndex = index;

        // SPRITE'LARI DEĞİŞTİR
        switch (index)
        {
            case 0:
                topRenderer.sprite = noneTop;
                torsoRenderer.sprite = noneTorso;
                bottomRenderer.sprite = noneBottom;
                break;
            case 1:
                topRenderer.sprite = akaiTop;
                torsoRenderer.sprite = akaiTorso;
                bottomRenderer.sprite = akaiBottom;
                break;
            case 2:
                topRenderer.sprite = shiroTop;
                torsoRenderer.sprite = shiroTorso;
                bottomRenderer.sprite = shiroBottom;
                break;
            case 3:
                topRenderer.sprite = mizumiTop;
                torsoRenderer.sprite = mizumiTorso;
                bottomRenderer.sprite = mizumiBottom;
                break;
            case 4:
                topRenderer.sprite = josephTop;
                torsoRenderer.sprite = josephTorso;
                bottomRenderer.sprite = josephBottom;
                break;
        }

        // PLAYABLE'LARI GÜNCELLE
        SetPlayablesForCostume(index);

        topTimer = torsoTimer = bottomTimer = 0f;
    }

    void PlayPunch()
    {
        if (isPunchPlaying) return;

        // Mevcut kostümün playable'larını aktif et
        mixer.SetInputWeight(0, 1f);
        mixer.SetInputWeight(1, 1f);
        mixer.SetInputWeight(2, 1f);

        // Playable'ları sıfırla ve oynat
        var clips = costumeClips[currentCostumeIndex];
        clipPlayables[clips[0]].SetTime(0);
        clipPlayables[clips[1]].SetTime(0);
        clipPlayables[clips[2]].SetTime(0);
        
        clipPlayables[clips[0]].Play();
        clipPlayables[clips[1]].Play();
        clipPlayables[clips[2]].Play();

        topTimer = torsoTimer = bottomTimer = 0f;
        isPunchPlaying = true;
    }

    void OnDestroy()
    {
        if (graph.IsValid())
        {
            graph.Destroy();
        }
    }
}