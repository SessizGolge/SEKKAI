using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSortHandler : MonoBehaviour
{
    public int offset = 0;
    private SpriteRenderer sr;

    // Ortadaki sorting değeri
    private int baseSorting = 1000;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Y pozisyonuna göre sıralama değeri hesapla
        sr.sortingOrder = baseSorting - Mathf.RoundToInt(transform.position.y * 100) + offset;
    }
}
