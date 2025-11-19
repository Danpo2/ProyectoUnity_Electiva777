using UnityEngine;
using System.Collections;

public class PigeonSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject pigeonPrefab;

    [Header("Cámara / Posición")]
    public Camera cam;                        // si lo dejas vacío, usa Camera.main
    public float spawnY = -2.57f;               // altura fija para todas las palomas
    public float offsetFromRight = 1.5f;      // cuánto fuera del borde derecho aparecen

    [Header("Frecuencia")]
    public Vector2 timeBetweenSpawns = new Vector2(4f, 7f); // aleatorio entre min y max
    public bool spawnOnStart = true;

    [Header("Velocidad")]
    public float pigeonSpeed = 8f;            // debe coincidir con tu velocidad de nivel
    public float speedMultiplier = 1f;        // para subir/bajar globalmente

    [Header("Z y Sorting")]
    public float z = 0f;                      // z where pigeons live (2D = 0)

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void OnEnable()
    {
        if (spawnOnStart)
            StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return null;

        while (true)
        {
            float wait = Random.Range(timeBetweenSpawns.x, timeBetweenSpawns.y);
            yield return new WaitForSeconds(wait);

            SpawnOne();
        }
    }

    public void SpawnOne()
    {
        if (!pigeonPrefab || !cam) return;

        // Borde derecho de la cámara en mundo
        float right = cam.transform.position.x + cam.orthographicSize * cam.aspect;

        Vector3 pos = new Vector3(right + offsetFromRight, spawnY, z);
        var go = Instantiate(pigeonPrefab, pos, Quaternion.identity, transform);

        // Configura movimiento
        var mover = go.GetComponent<PigeonMover>();
        if (mover)
        {
            mover.speed = pigeonSpeed;
            mover.speedMultiplier = speedMultiplier;
        }
    }
}
