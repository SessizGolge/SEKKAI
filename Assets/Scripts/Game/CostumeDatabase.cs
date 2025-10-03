using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CostumeInfo
{
    public string costumeName;
    public int cost;
}

[CreateAssetMenu(fileName = "CostumeDatabase", menuName = "Game/Costume Database")]
public class CostumeDatabase : ScriptableObject
{
    public List<Sprite> tops;
    public List<Sprite> torsos;
    public List<Sprite> bottoms;

    public List<AnimationClip> topWalkClips;
    public List<AnimationClip> torsoWalkClips;
    public List<AnimationClip> bottomWalkClips;

    public List<AnimationClip> topAttackClips;
    public List<AnimationClip> torsoAttackClips;

    public List<CostumeInfo> costumes;

    public List<string> costumeNames
    {
        get
        {
            List<string> names = new List<string>();
            foreach (var c in costumes)
                names.Add(c.costumeName);
            return names;
        }
    }

    public List<int> costumeCost
    {
        get
        {
            List<int> costs = new List<int>();
            foreach (var c in costumes)
                costs.Add(c.cost);
            return costs;
        }
    }
}
