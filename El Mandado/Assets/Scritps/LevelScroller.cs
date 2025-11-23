using UnityEngine;
using UnityEngine.Events;

public class LevelScroller : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        public Transform root;          // GameObject padre de la capa (piso, casas, fondo)
        [Range(0f, 1f)]
        public float parallax = 1f;     // 1 = se mueve igual que el nivel, 0.3 = más lento (fondo lejano)
    }

    [Header("Capas del nivel (de cerca a lejos)")]
    public Layer[] layers;

    [Header("Movimiento")]
    [Tooltip("Velocidad base hacia la izquierda (unidades/segundo).")]
    public float baseSpeed = 6f;
    [Tooltip("Usa deltaTime normal (se pausa con Time.timeScale). Si quieres ignorar la pausa, usa unscaledDeltaTime.")]
    public bool ignorePause = false;

    [Header("Fin del nivel")]
    [Tooltip("Cámara que se usa para calcular cuándo el fondo se acabó en pantalla. Si se deja vacío, usa Camera.main")]
    public Camera cam;
    [Tooltip("Margen extra al calcular cuando el último pixel del fondo sale por la izquierda.")]
    public float endMargin = 0.25f;
    public UnityEvent onLevelFinished;  // lo que quieras hacer al terminar (mostrar score, cargar escena, etc.)

    // --- internos ---
    Vector3[] startPos;     // posición inicial de cada capa
    float minX, maxX;       // bounds globales del nivel al inicio (en mundo)
    Vector3 rootStart;      // posición inicial del objeto que lleva este script

    void Awake()
    {
        if (!cam) cam = Camera.main;

        // Guardar posiciones iniciales de cada capa
        startPos = new Vector3[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].root)
                startPos[i] = layers[i].root.position;
        }

        rootStart = transform.position;

        // Calcular bounds globales del nivel (minX, maxX) encapculando todos los Renderers hijos
        bool hasAny = false;
        Bounds b = new Bounds(Vector3.zero, Vector3.zero);
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (!hasAny)
            {
                b = r.bounds;
                hasAny = true;
            }
            else
            {
                b.Encapsulate(r.bounds);
            }
        }

        if (!hasAny)
        {
            Debug.LogWarning("[LevelScroller] No encontré Renderers para medir el ancho del nivel. Revisa la jerarquía.");
            // como fallback, considera 100 unidades de ancho
            minX = transform.position.x - 50f;
            maxX = transform.position.x + 50f;
        }
        else
        {
            minX = b.min.x;
            maxX = b.max.x;
        }
    }

    void Update()
    {
        float dt = ignorePause ? Time.unscaledDeltaTime : Time.deltaTime;
        if (dt <= 0f) return; // pausado

        // Mover cada capa con su parallax
        for (int i = 0; i < layers.Length; i++)
        {
            if (!layers[i].root) continue;
            float v = baseSpeed * layers[i].parallax;
            layers[i].root.position += Vector3.left * v * dt;
        }

        // ¿Se acabó el nivel? (cuando el borde derecho ya salió por el lado izquierdo de la cámara)
        
    }

    int GetNearestLayerIndex()
    {
        // intenta usar la capa con parallax más cercano a 1 (la que “avanza” a velocidad del mundo)
        int idx = -1;
        float best = -1f;
        for (int i = 0; i < layers.Length; i++)
        {
            if (!layers[i].root) continue;
            float score = 1f - Mathf.Abs(1f - layers[i].parallax);
            if (score > best) { best = score; idx = i; }
        }
        return idx;
    }

    // --- API opcional ---
    public void SetSpeed(float newSpeed) => baseSpeed = newSpeed;
    public void PauseScroll(bool pause)
    {
        if (pause) { /* con Time.timeScale=0 ya se pausa; si usas ignorePause=true, desactiva script: */ if (ignorePause) enabled = false; }
        else { enabled = true; }
    }
}
