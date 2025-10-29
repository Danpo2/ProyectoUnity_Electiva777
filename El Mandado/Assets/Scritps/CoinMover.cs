using UnityEngine;

public class CoinMover : MonoBehaviour
{
    [Tooltip("Velocidad hacia la izquierda (u/s).")]
    public float speed = 6f;

    [Tooltip("Si true, se moverá incluso cuando el juego esté pausado.")]
    public bool ignorePause = false;

    [Tooltip("Destruir cuando pase este margen más allá del borde izquierdo de cámara.")]
    public float destroyMargin = 1f;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        float dt = ignorePause ? Time.unscaledDeltaTime : Time.deltaTime;
        if (dt <= 0f) return;

        transform.position += Vector3.left * speed * dt;

        if (cam)
        {
            float left = cam.transform.position.x - cam.orthographicSize * cam.aspect - destroyMargin;
            if (transform.position.x < left)
                Destroy(gameObject);
        }
    }
}
