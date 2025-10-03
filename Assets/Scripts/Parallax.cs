using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;   // Main Camera
    public Transform background;        // Arkadaki sabit background

    [Header("Parallax Layers")]
    public Transform[] parallaxLayers;  // 3 layer
    public float[] parallaxFactors;     // Her layer için hız (0 = sabit, 1 = kamera ile aynı)

    private Vector3 lastCameraPos;
    private float backgroundOffsetX;

    [Header("Pixel Perfect")]
    public float pixelPerUnit = 32f; // Spriteların PPU değeri ile aynı olmalı

    [Header("Background Clamp")]
    public float minX = -18f;
    public float maxX = 23.7f;

    void Start()
    {
        lastCameraPos = cameraTransform.position;

        if (background != null)
        {
            backgroundOffsetX = background.position.x - cameraTransform.position.x;
        }
    }

    void LateUpdate()
    {
        // 1) Kamera pozisyonunu yuvarla → titreme önlenir
        Vector3 camPos = cameraTransform.position;
        camPos.x = Mathf.Round(camPos.x * pixelPerUnit) / pixelPerUnit;

        // 2) Background → kamera ile aynı X hareketini yap ama clamp'li, Y sabit kalsın
        if (background != null)
        {
            Vector3 targetPos = background.position;
            targetPos.x = Mathf.Clamp(camPos.x + backgroundOffsetX, minX, maxX);
            background.position = targetPos;
        }

        // 3) Parallax katmanlar → sadece X ekseninde hareket
        Vector3 delta = camPos - lastCameraPos;

        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i] != null && i < parallaxFactors.Length)
            {
                Vector3 pos = parallaxLayers[i].position;
                pos.x += delta.x * parallaxFactors[i]; // sadece X hareket
                parallaxLayers[i].position = pos;
            }
        }

        lastCameraPos = camPos;
    }
}
