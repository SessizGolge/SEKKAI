using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerCostumeManager : MonoBehaviour
{
    [Header("SpriteRenderers")]
    [SerializeField] private SpriteRenderer topVFX;
    [SerializeField] private SpriteRenderer torsoVFX;
    [SerializeField] private SpriteRenderer bottomVFX;

    [Header("Animator")]
    [SerializeField] private Animator playerAnimator;

    [Header("Costume Database")]
    [SerializeField] private CostumeDatabase database;

    private AnimatorOverrideController overrideController;

    void Awake()
    {
        overrideController = new AnimatorOverrideController(playerAnimator.runtimeAnimatorController);
        playerAnimator.runtimeAnimatorController = overrideController;
    }

    public void SetCostume(int topIndex, int torsoIndex, int bottomIndex)
    {
        StartCoroutine(ApplySpritesNextFrame(topIndex, torsoIndex, bottomIndex));
        ApplyAnimationOverrides(topIndex, torsoIndex, bottomIndex);
    }

    private IEnumerator ApplySpritesNextFrame(int topIndex, int torsoIndex, int bottomIndex)
    {
        yield return null; // 1 frame bekle
        topVFX.sprite = database.tops[topIndex];
        torsoVFX.sprite = database.torsos[torsoIndex];
        bottomVFX.sprite = database.bottoms[bottomIndex];
    }

    private void ApplyAnimationOverrides(int topIndex, int torsoIndex, int bottomIndex)
    {
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.topWalkClips[0], database.topWalkClips[topIndex]));
        overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.torsoWalkClips[0], database.torsoWalkClips[torsoIndex]));
        overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.bottomWalkClips[0], database.bottomWalkClips[bottomIndex]));

        if (database.topAttackClips.Count > topIndex)
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.topAttackClips[0], database.topAttackClips[topIndex]));
        if (database.torsoAttackClips.Count > torsoIndex)
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(database.torsoAttackClips[0], database.torsoAttackClips[torsoIndex]));

        overrideController.ApplyOverrides(overrides);
    }
}
