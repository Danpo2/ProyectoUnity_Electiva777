using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    [Tooltip("Cuántas monedas vale este pickup.")]
    public int value = 1;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) { col.isTrigger = true; }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Sumar en HUD
        HUDGameUI.I?.AddCoins(value * 1000);

        // TODO: Efecto/SFX opcional aquí (partículas, sonido)

        // Destruir moneda
        Destroy(gameObject);
    }
}
