using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraConfiner2D : MonoBehaviour
{
    [Header("Confiner Settings")]
    public PolygonCollider2D confinerCollider; // Sınır poligon
    public float smoothTime = 0.2f; // Kamera geçişlerinin yumuşaklığı

    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (confinerCollider == null) return;

        // Kameranın görünür alanını hesaba kat
        Vector2 cameraSize = new Vector2(
            cam.orthographicSize * cam.aspect,
            cam.orthographicSize
        );

        Vector3 targetPos = transform.position;

        // Kamera merkezini sınır poligonunun içine kısıtla
        Vector2 clampedPos = ClampToPolygon(targetPos, cameraSize);

        // Yumuşak geçiş
        transform.position = Vector3.SmoothDamp(
            transform.position,
            new Vector3(clampedPos.x, clampedPos.y, targetPos.z),
            ref velocity,
            smoothTime
        );
    }

    Vector2 ClampToPolygon(Vector3 targetPos, Vector2 camSize)
    {
        Vector2 pos2D = targetPos;

        // Nokta poligonun içinde mi?
        if (confinerCollider.OverlapPoint(pos2D))
        {
            return pos2D; // İçerideyse direkt kullan
        }

        // Dışarıdaysa → poligonun en yakın noktasına sıkıştır
        Vector2 closest = confinerCollider.ClosestPoint(pos2D);
        return closest;
    }
}
