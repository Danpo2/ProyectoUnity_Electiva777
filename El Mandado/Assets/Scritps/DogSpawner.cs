using UnityEngine;
using System.Collections;

public class DogSpawner : MonoBehaviour
{
    public GameObject dogPrefab;
    public Camera cam;
    public Transform player;      // referencia al jugador
    public float yOffset = 0f;    // ajuste fino si hace falta (ej: -0.1f)

    public float offsetFromRight = 1.2f;
    public float z = 0f;

    [Header("Frecuencia")]
    [Tooltip("Tiempo entre spawns (segundos).")]
    public float spawnInterval = 2f;
    public bool spawnOnStart = true;

    [Header("Movimiento del perro")]
    [Tooltip("Velocidad hacia la izquierda que tendrá el perro (si usa DogMover).")]
    public float dogSpeed = 4.5f;

    Coroutine loop;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void OnEnable()
    {
        if (spawnOnStart && loop == null)
            loop = StartCoroutine(SpawnLoop());
    }

    void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
    }

    IEnumerator SpawnLoop()
    {
        yield return null; // evita spawnear en frame 0

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnOne();
        }
    }

    [ContextMenu("Spawn One (Test)")]
    public void SpawnOne()
    {
        if (!dogPrefab || !cam || !player) return;

        float right = cam.transform.position.x + cam.orthographicSize * cam.aspect;

        float spawnY = player.position.y + yOffset;   // misma altura que el jugador
        Vector3 pos = new Vector3(right + offsetFromRight, spawnY, z);

        GameObject go = Instantiate(dogPrefab, pos, Quaternion.identity);

        var mover = go.GetComponent<DogMover>();
        if (mover) mover.speed = dogSpeed;
    }
}
