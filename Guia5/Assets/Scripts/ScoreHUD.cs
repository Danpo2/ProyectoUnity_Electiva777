using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Linq;

public class ScoreHUD : MonoBehaviour
{
    [SerializeField] private Text myScoreText;
    [SerializeField] private Text enemyScoreText;

    private PlayerState myState;

    private void Start()
    {
        // Espera a que exista mi Player
        NetworkManager.Singleton.OnClientConnectedCallback += _ => TryHook();
        NetworkManager.Singleton.OnServerStarted += TryHook;
        TryHook();
    }

    private void TryHook()
    {
        if (myState != null) return;

        var myClientId = NetworkManager.Singleton.LocalClientId;
        var myPlayer = NetworkManager.Singleton.ConnectedClients.TryGetValue(myClientId, out var client)
            ? client.PlayerObject
            : null;

        if (myPlayer != null)
        {
            myState = myPlayer.GetComponent<PlayerState>();
            if (myState != null)
            {
                myState.Score.OnValueChanged += (_, __) => Refresh();
                Refresh();
            }
        }
    }

    private void Update()
    {
        // Actualiza el puntaje enemigo cada frame de forma simple (o haz un timer)
        Refresh();
    }

    private void Refresh()
    {
        if (myState && myScoreText) myScoreText.text = $"Yo: {myState.Score.Value}";

        // “Enemigo” = cualquier otro PlayerState que no sea el mío (para 1v1)
        var others = NetworkManager.Singleton.ConnectedClientsList
            .Where(c => c.ClientId != NetworkManager.Singleton.LocalClientId)
            .Select(c => c.PlayerObject?.GetComponent<PlayerState>())
            .Where(ps => ps != null);

        int enemyScore = others.FirstOrDefault()?.Score.Value ?? 0;
        if (enemyScoreText) enemyScoreText.text = $"Enemigo: {enemyScore}";
    }
}
