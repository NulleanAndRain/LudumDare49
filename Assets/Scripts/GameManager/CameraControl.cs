using UnityEngine;

public class CameraControl : MonoBehaviour {
    private static Camera mainCamera;
    public GameObject player;
    public Vector2 offset;
    public bool useBorders;

    public float leftBorder;
    public float rightBorder;
    public float topBorder;
    public float bottomBorder;

    private float moveTime = 0;

    public static bool isMoving { get; set; } = true;

    void Start() {
        mainCamera = GetComponent<Camera>();
        _pos = transform.position;
    }

    private Vector3 _pos = new Vector3(0, 0, -10);
    void Update() {
        if (!isMoving || PauseControl.isPaused) return;
        moveTime %= Time.fixedDeltaTime;
        moveTime += Time.deltaTime * 3;

        _pos = Vector2.Lerp(
            mainCamera.transform.position,
            (Vector2)player.transform.position + offset,
            moveTime
            );
        _pos.z = -10;

        if (useBorders) {
            _pos.y = Mathf.Clamp(_pos.y, bottomBorder, topBorder);
            _pos.x = Mathf.Clamp(_pos.x, leftBorder, rightBorder);
        }
        mainCamera.transform.position = _pos;
    }

}