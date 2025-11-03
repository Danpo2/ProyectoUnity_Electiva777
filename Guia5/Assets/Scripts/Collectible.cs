using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject), typeof(Collider))]
public class Collectible : NetworkBehaviour
{
    private bool taken = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || taken) return; // solo el servidor procesa la recolección

        // ¿Entró un jugador?
        var netObj = other.GetComponentInParent<NetworkObject>();
        if (netObj == null || !netObj.IsPlayerObject) return;

        var state = other.GetComponentInParent<PlayerState>();
        if (state == null) return;

        taken = true; // evita dobles toques en el mismo frame

        // Sumar puntaje en el servidor
        state.AddScoreServerRpc(1);

        // Despawnear el ítem para todos
        GetComponent<NetworkObject>().Despawn(true);
    }
}
