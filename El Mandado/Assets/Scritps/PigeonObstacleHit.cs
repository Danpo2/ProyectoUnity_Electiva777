using UnityEngine;
using UnityEngine.Events;

public class PigeonObstacleHit : MonoBehaviour
{
    [Tooltip("Se invoca si el jugador NO está deslizándose al chocar")]
    public UnityEvent onPlayerHit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var anim = other.GetComponentInChildren<Animator>();
        bool isSliding = anim && anim.GetBool("Crouch");

        if (isSliding)
        {
            // pasa sin consecuencias
            return;
        }

        // Evento opcional (sonido, partículas, etc.)
        onPlayerHit?.Invoke();

        // Avisar al GameManager que perdió
        GameManager.I?.PlayerCrashed();
    }
}
