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
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Evento opcional (sonido, cámara shake, etc.)
        onPlayerHit?.Invoke();

        // Avisar al GameManager que perdió
        GameManager.I?.PlayerCrashed();

        Debug.Log("💥 Player golpeó al perro.");
    }
}
