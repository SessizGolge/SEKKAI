using UnityEngine;

public class CameraLimits : MonoBehaviour 
{
    public new Camera camera;
	public Transform limitLeft;
	public Transform limitRight;
	public Transform limitTop;
	public Transform limitBottom;

	void Update () {
		float vertical = camera.orthographicSize;
		float horizontal = vertical * Screen.width / Screen.height;

		Vector3 position = transform.position;

		position.x = Mathf.Clamp(position.x, limitLeft.position.x + horizontal, limitRight.position.x - horizontal);
		position.y = Mathf.Clamp(position.y, limitTop.position.y + vertical, limitBottom.position.y - vertical);

		transform.position = position;
	}
}
