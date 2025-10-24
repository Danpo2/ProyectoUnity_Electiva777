using UnityEngine;
using UnityEngine.Events;

public class PigeonObstacleHit : MonoBehaviour
{
    [Tooltip("Se invoca si el jugador NO está deslizándose al chocar")]
    public UnityEvent onPlayerHit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 1) Intentar leer estado desde el Animator ("Crouch" debe ser true al deslizar)
        var anim = other.GetComponentInChildren<Animator>();
        bool isSliding = anim && anim.GetBool("Crouch");

        // 2) Si tienes un método público en tu controller, úsalo (opcional)
        // var ctrl = other.GetComponentInParent<PlayerRunnerController>();
        // if (ctrl) isSliding = ctrl.IsSliding();

        if (isSliding)
        {
            // Pasa por debajo sin consecuencias
            return;
        }

        // No está deslizándose → golpe
        onPlayerHit?.Invoke();

        // Si quieres destruir la paloma al golpear:
        // Destroy(gameObject);
    }
}
