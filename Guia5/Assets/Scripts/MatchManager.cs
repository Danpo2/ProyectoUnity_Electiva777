using UnityEngine;
using Unity.Netcode;

public class MatchManager : NetworkBehaviour
{
    [SerializeField] private float matchDurationSec = 120f; // 2 minutos
    public NetworkVariable<float> TimeRemaining = new NetworkVariable<float>();
    public NetworkVariable<bool> IsRunning = new NetworkVariable<bool>(false);

    private float serverTimer;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            serverTimer = matchDurationSec;
            TimeRemaining.Value = matchDurationSec;
            IsRunning.Value = true;
        }
    }

    private void Update()
    {
        if (!IsServer || !IsRunning.Value) return;

        serverTimer -= Time.deltaTime;
        TimeRemaining.Value = Mathf.Max(0f, serverTimer);

        if (serverTimer <= 0f)
        {
            IsRunning.Value = false;
            AnnounceWinner();
        }
    }
    private void LockAllPlayersInput(bool locked)
    {
        // Solo el servidor debe orquestar esto
        if (!IsServer) return;

        foreach (var c in NetworkManager.Singleton.ConnectedClientsList)
        {
            var pc = c.PlayerObject ? c.PlayerObject.GetComponent<PlayerController>() : null;
            if (pc != null)
            {
                // Desactiva el componente de control como “bloqueo” sencillo
                pc.enabled = !locked;
            }
        }
    }

    private void AnnounceWinner()
    {
        // 1v1: compara los dos PlayerState
        int bestScore = int.MinValue;
        ulong bestClient = ulong.MaxValue;
        bool tie = false;

        foreach (var c in NetworkManager.Singleton.ConnectedClientsList)
        {
            var ps = c.PlayerObject?.GetComponent<PlayerState>();
            if (ps == null) continue;

            if (ps.Score.Value > bestScore)
            {
                bestScore = ps.Score.Value;
                bestClient = c.ClientId;
                tie = false;
            }
            else if (ps.Score.Value == bestScore)
            {
                tie = true; // empate
            }
        }

        if (tie) ShowEndMessageClientRpc("Empate");
        else
        {
            string msg = (bestClient == NetworkManager.Singleton.LocalClientId) ? "Ganaste" : "Perdiste";
            // Ojo: el mensaje anterior solo tiene sentido localmente; mejor mandamos el clientId del ganador:
            ShowEndMessageWithWinnerClientRpc(bestClient);
        }
    }

    [ClientRpc]
    private void ShowEndMessageClientRpc(string message)
    {
#if UNITY_2023_1_OR_NEWER
    var hud = Object.FindFirstObjectByType<MatchHUD>();
#else
        var hud = Object.FindObjectOfType<MatchHUD>();
#endif
        if (hud) hud.ShowEnd(message);

        // O elimínala o define el helper como se muestra arriba
        // LockAllPlayersInput(true);
    }

    [ClientRpc]
    private void ShowEndMessageWithWinnerClientRpc(ulong winnerClientId)
    {
#if UNITY_2023_1_OR_NEWER
    var hud = Object.FindFirstObjectByType<MatchHUD>();
#else
        var hud = Object.FindObjectOfType<MatchHUD>();
#endif
        if (!hud) return;

        if (NetworkManager.Singleton.LocalClientId == winnerClientId) hud.ShowEnd("¡Ganaste!");
        else hud.ShowEnd("Perdiste");
    }

}
