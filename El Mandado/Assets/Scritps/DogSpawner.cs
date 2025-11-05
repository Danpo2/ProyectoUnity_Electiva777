using UnityEngine;
using System.Collections;

public class DogSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject dogPrefab;

    [Header("Posición")]
    public Camera cam;
    [Tooltip("Altura fija en Y para el perro (suelo).")]
    public float spawnY = -2.5f;
    [Tooltip("Cuánto fuera del borde derecho aparece.")]
    public float offsetFromRight = 1.2f;
    [Tooltip("Z de la instancia (2D = 0).")]
    public float z = 0f;

    [Header("Frecuencia")]
    [Tooltip("Tiempo entre spawns (segundos).")]
    public float spawnInterval = 5f;
    public bool spawnOnStart = true;

    [Header("Movimiento del perro")]
    [Tooltip("Velocidad hacia la izquierda que tendrá el perro (si usa DogMover).")]
    public float dogSpeed = 6f;

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
        if (!dogPrefab || !cam) return;

        float right = cam.transform.position.x + cam.orthographicSize * cam.aspect;
        Vector3 pos = new Vector3(right + offsetFromRight, spawnY, z);

        // Instanciar conservando la escala del prefab
        GameObject go = Instantiate(dogPrefab, pos, Quaternion.identity);

        // Configurar movimiento si tiene DogMover
        var mover = go.GetComponent<DogMover>();
        if (mover) mover.speed = dogSpeed;
    }
}
