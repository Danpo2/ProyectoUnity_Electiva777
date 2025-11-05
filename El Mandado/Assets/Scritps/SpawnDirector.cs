using UnityEngine;
using System.Collections;

/// <summary>
/// Coordina los spawns de perro (salto) y paloma (desliz) para evitar conflictos imposibles.
/// Desactiva los bucles internos de los spawners: spawnOnStart = false en ambos.
/// </summary>
public class SpawnDirector : MonoBehaviour
{
    [Header("Referencias")]
    public DogSpawner dogSpawner;            // Asigna tu DogSpawner en escena
    public PigeonSpawner pigeonSpawner;      // Asigna tu PigeonSpawner en escena
    public Transform player;                 // Player en el mundo (NO dentro del Canvas)
    public Camera cam;                       // Main Camera (si se deja vacío se usa Camera.main)

    [Header("Intervalos (segundos)")]
    public Vector2 dogInterval = new Vector2(4f, 7f);       // rango aleatorio entre perros
    public Vector2 pigeonInterval = new Vector2(3f, 6f);    // rango aleatorio entre palomas

    [Header("Reglas de conflicto")]
    [Tooltip("Separación mínima entre tiempos de llegada al jugador (s).")]
    public float minConflictGap = 1.25f;

    [Tooltip("Margen extra por si las velocidades varían un poco en runtime.")]
    public float arrivalTolerance = 0.05f;

    // tracking de último ETA de cada tipo
    float lastDogArrival = -999f;
    float lastPigeonArrival = -999f;

    Coroutine dogLoop, pigeonLoop;

    void Awake()
    {
        if (!cam) cam = Camera.main;
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
        // pequeño delay para evitar spawns en el frame 0
        yield return null;

        while (true)
        {
            float wait = Random.Range(dogInterval.x, dogInterval.y);
            yield return new WaitForSeconds(wait);

            // Estimar ETA del perro SI lo spawneamos AHORA
            float dogETA = EstimateArrivalSeconds(
                speed: dogSpawner.dogSpeed,
                offsetFromRight: dogSpawner.offsetFromRight
            );

            // ¿Choca con una paloma próxima?
            if (Mathf.Abs(dogETA - lastPigeonArrival) < (minConflictGap - arrivalTolerance))
            {
                // Esperar lo necesario para separarlo
                float extra = (minConflictGap - Mathf.Abs(dogETA - lastPigeonArrival));
                if (extra > 0f) yield return new WaitForSeconds(extra);
            }

            // Spawn y confirma ETA definitivo (por si la cámara se movió un poco)
            dogSpawner.SpawnOne();
            lastDogArrival = EstimateArrivalSeconds(
                speed: dogSpawner.dogSpeed,
                offsetFromRight: dogSpawner.offsetFromRight
            );
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
                speed: pigeonSpawner.pigeonSpeed,
                offsetFromRight: pigeonSpawner.offsetFromRight
            );

            if (Mathf.Abs(pigeonETA - lastDogArrival) < (minConflictGap - arrivalTolerance))
            {
                float extra = (minConflictGap - Mathf.Abs(pigeonETA - lastDogArrival));
                if (extra > 0f) yield return new WaitForSeconds(extra);
            }

            pigeonSpawner.SpawnOne();
            lastPigeonArrival = EstimateArrivalSeconds(
                speed: pigeonSpawner.pigeonSpeed,
                offsetFromRight: pigeonSpawner.offsetFromRight
            );
        }
    }

    float EstimateArrivalSeconds(float speed, float offsetFromRight)
    {
        if (!cam || !player) return 999f;

        float right = cam.transform.position.x + cam.orthographicSize * cam.aspect;
        float spawnX = right + offsetFromRight;
        float px = player.position.x;

        float dist = spawnX - px;
        float v = Mathf.Max(0.01f, speed);
        return dist / v;
    }
}
