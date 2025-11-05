using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class DogObstacleHit : MonoBehaviour
{
    [Tooltip("Se invoca cuando el jugador toca al perro (pierde).")]
    public UnityEvent onPlayerHit;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true; // trabajamos con trigger
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Pierde al tocar el perro (independiente de si está saltando)
        Time.timeScale = 0f;                  // pausa el juego
        onPlayerHit?.Invoke();                // para abrir UI, sonidos, etc.
        Debug.Log("💥 Player golpeó al perro. Juego detenido.");
    }
}
