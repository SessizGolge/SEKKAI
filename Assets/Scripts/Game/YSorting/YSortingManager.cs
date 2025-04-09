using UnityEngine;

public class YSortManager : MonoBehaviour
{
    void Start()
    {
        ApplyYSorting();
    }

    void ApplyYSorting()
    {
        YSortHandler[] ySortedObjects = FindObjectsOfType<YSortHandler>();

        foreach (YSortHandler ysh in ySortedObjects)
        {
            // Bu zaten YSortHandler içinde yapılacak ama güvenli olsun dersen yine de forcing:
            ysh.GetComponent<SpriteRenderer>().sortingOrder =
                1000 - Mathf.RoundToInt(ysh.transform.position.y * 100) + ysh.offset;
        }
    }
}
