using UnityEngine;

public class PigeonMover : MonoBehaviour
{
    [Tooltip("Velocidad base hacia la izquierda (unidades/segundo)")]
    public float speed = 6f;

    [Tooltip("Si el runner acelera, puedes multiplicar desde fuera")]
    public float speedMultiplier = 1f;

    [Tooltip("Ignorar la pausa. Si es false, el pájaro se detiene cuando Time.timeScale=0")]
    public bool ignorePause = false;

    [Tooltip("Cuánto más allá del borde izquierdo destruir el objeto")]
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

        transform.position += Vector3.left * speed * speedMultiplier * dt;

        if (cam)
        {
            float left = cam.transform.position.x - cam.orthographicSize * cam.aspect - destroyMargin;
            if (transform.position.x < left)
                Destroy(gameObject);
        }
    }
}
