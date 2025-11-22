using UnityEngine;
using System.Collections;

public class SpawnDirector : MonoBehaviour
{
    [Header("Referencias")]
    public DogSpawner dogSpawner;
    public PigeonSpawner pigeonSpawner;
    public Camera cam;
    public Transform player;        // <--- NUEVO
    public float fixedPlayerX = -4.38f;

    [Header("Intervalos (segundos)")]
    public Vector2 dogInterval = new Vector2(4f, 7f);
    public Vector2 pigeonInterval = new Vector2(3f, 6f);

    [Header("Reglas de conflicto")]
    [Tooltip("Separación mínima entre tiempos de llegada al jugador (s).")]
    public float minConflictGap = 1.25f;

    [Tooltip("Margen extra por si las velocidades varían un poco en runtime.")]
    public float arrivalTolerance = 0.05f;

    // Últimos tiempos estimados de llegada
    float lastDogArrival = -999f;
    float lastPigeonArrival = -999f;

    Coroutine dogLoop;
    Coroutine pigeonLoop;

    void Awake()
    {
        if (dogSpawner) dogSpawner.player = player;
        if (pigeonSpawner) pigeonSpawner.player = player;
    }

    void OnEnable()
    {
        if (dogLoop == null) dogLoop = StartCoroutine(DogLoop());
        if (pigeonLoop == null) pigeonLoop = StartCoroutine(PigeonLoop());
    }

    void OnDisable()
    {
        if (dogLoop != null) StopCoroutine(dogLoop);
        if (pigeonLoop != null) StopCoroutine(pigeonLoop);
        dogLoop = pigeonLoop = null;
    }

    IEnumerator DogLoop()
    {
        yield return null;

        while (true)
        {
            float wait = Random.Range(dogInterval.x, dogInterval.y);
            yield return new WaitForSeconds(wait);

            float dogETA = EstimateArrivalSeconds(
                dogSpawner.dogSpeed,
                dogSpawner.offsetFromRight
            );

            float diff = Mathf.Abs(dogETA - lastPigeonArrival);

            if (diff < minConflictGap)
            {
                float extraWait = minConflictGap - diff;
                yield return new WaitForSeconds(extraWait);
                dogETA = EstimateArrivalSeconds(dogSpawner.dogSpeed, dogSpawner.offsetFromRight);
            }

            dogSpawner.SpawnOne();
            lastDogArrival = dogETA;
        }
    }

    IEnumerator PigeonLoop()
    {
        yield return null;

        while (true)
        {
            float wait = Random.Range(pigeonInterval.x, pigeonInterval.y);
            yield return new WaitForSeconds(wait);

            float pigeonETA = EstimateArrivalSeconds(
                pigeonSpawner.pigeonSpeed,
                pigeonSpawner.offsetFromRight
            );

            float diff = Mathf.Abs(pigeonETA - lastDogArrival);

            if (diff < minConflictGap)
            {
                float extraWait = minConflictGap - diff;
                yield return new WaitForSeconds(extraWait);
                pigeonETA = EstimateArrivalSeconds(pigeonSpawner.pigeonSpeed, pigeonSpawner.offsetFromRight);
            }

            pigeonSpawner.SpawnOne();
            lastPigeonArrival = pigeonETA;
        }
    }

    float EstimateArrivalSeconds(float speed, float offsetFromRight)
    {
        if (!cam)
        {
            Debug.LogWarning("SpawnDirector: cámara no asignada.");
            return 999f;
        }

        float right = cam.transform.position.x + cam.orthographicSize * cam.aspect;
        float spawnX = right + offsetFromRight;

        float dist = spawnX - fixedPlayerX; // Usa posición fija del jugador

        float v = Mathf.Max(0.01f, speed);

        return dist / v;
    }
}
