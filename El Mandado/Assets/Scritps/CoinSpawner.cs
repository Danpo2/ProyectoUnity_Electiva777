using UnityEngine;
using System.Collections;

public class CoinSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject coinPrefab;

    [Header("Posición")]
    public Camera cam;
    [Tooltip("Altura fija en Y para las monedas.")]
    public float spawnY = -2.5f;
    [Tooltip("Cuánto fuera del borde derecho aparecen.")]
    public float offsetFromRight = 1.2f;
    [Tooltip("Z de la moneda (2D = 0).")]
    public float z = 0f;

    [Header("Frecuencia")]
    [Tooltip("Tiempo entre spawns (segundos).")]
    public float spawnInterval = 7f;
    public bool spawnOnStart = true;

    [Header("Movimiento de la moneda")]
    [Tooltip("Velocidad hacia la izquierda que tendrán las monedas instanciadas (si usan CoinMover).")]
    public float coinSpeed = 6f;

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
        // Pequeño delay para evitar spawnear en el frame 0
        yield return null;

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnOne();
        }
    }

    [ContextMenu("Spawn One (Test)")]
    public void SpawnOne()
    {
        if (!coinPrefab || !cam) return;

        float right = cam.transform.position.x + cam.orthographicSize * cam.aspect;
        Vector3 pos = new Vector3(right + offsetFromRight, spawnY, z);

        // ANTES: sin padre (monedas quedan sueltas en la escena)
        // GameObject go = Instantiate(coinPrefab, pos, Quaternion.identity);

        // AHORA: que cuelguen del propio CoinSpawner
        GameObject go = Instantiate(coinPrefab, pos, Quaternion.identity, transform);

        var mover = go.GetComponent<CoinMover>();
        if (mover) mover.speed = coinSpeed;
    }


}

