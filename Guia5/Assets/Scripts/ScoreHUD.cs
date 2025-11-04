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
        // Evita correr si aún no hay NetworkManager
        if (NetworkManager.Singleton == null) return;
        Refresh();
    }


    private void Refresh()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null || !nm.IsListening) return; // NM no está listo

        if (myState != null && myScoreText)
            myScoreText.text = $"Yo: {myState.Score.Value}";

        // “Enemigo” = cualquier otro PlayerState que no sea el mío (para 1v1)
        var others = nm.ConnectedClientsList
            .Where(c => c.ClientId != nm.LocalClientId)
            .Select(c => c.PlayerObject ? c.PlayerObject.GetComponent<PlayerState>() : null)
            .Where(ps => ps != null);

        int enemyScore = 0;
        var first = others.FirstOrDefault();
        if (first != null) enemyScore = first.Score.Value;

        if (enemyScoreText) enemyScoreText.text = $"Enemigo: {enemyScore}";
    }

}
