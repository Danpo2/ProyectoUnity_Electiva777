using UnityEngine;
using System.Collections;

public class PigeonSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject pigeonPrefab;

    [Header("Cámara / Posición")]
    public Camera cam;
    public Transform player;
    public float heightAbovePlayer = 1.2f;
    public float offsetFromRight = 1.5f;

    [Header("Frecuencia")]
    public Vector2 timeBetweenSpawns = new Vector2(4f, 7f);
    public bool spawnOnStart = true;

    [Header("Velocidad")]
    public float pigeonSpeed = 8f;
    public float speedMultiplier = 1f;

    [Header("Z y Sorting")]
    public float z = 0f;
    public PlayerRunnerController playerCtrl;
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
        if (!pigeonPrefab || !cam || !playerCtrl) return;

        float right = cam.transform.position.x + cam.orthographicSize * cam.aspect;
        float baseY = playerCtrl.GroundY;
        float spawnY = baseY + heightAbovePlayer;

        Vector3 pos = new Vector3(right + offsetFromRight, spawnY, z);
        var go = Instantiate(pigeonPrefab, pos, Quaternion.identity, transform);

        var mover = go.GetComponent<PigeonMover>();
        if (mover)
        {
            mover.speed = pigeonSpeed;
            mover.speedMultiplier = speedMultiplier;
        }
    }
}

