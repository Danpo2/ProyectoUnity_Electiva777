using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkLobbyUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyPanel;          // Panel raíz del lobby
    [SerializeField] private InputField ipInput;             // "127.0.0.1" o IP LAN del host
    [SerializeField] private InputField portInput;           // "7777"
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button shutdownButton;
    [SerializeField] private Text statusText;

    [Header("Juego")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private int minPlayersToStart = 2;      // Host + 1 Cliente

    private NetworkManager nm;
    private UnityTransport transport;

    // En NetworkLobbyUI
    private void Awake()
    {
        nm = NetworkManager.Singleton;
        if (!nm) TryFindNM();
        if (!nm) { Debug.LogError("Falta NetworkManager en la escena."); enabled = false; return; }

        transport = nm.GetComponent<UnityTransport>();
        if (!transport) { Debug.LogError("Falta UnityTransport en el NetworkManager."); enabled = false; return; }
    }

    private void TryFindNM()
    {
#if UNITY_2023_1_OR_NEWER
    var found = Object.FindFirstObjectByType<NetworkManager>();
#else
        var found = Object.FindObjectOfType<NetworkManager>();
#endif
        if (found) nm = found;
    }


    private void OnEnable()
    {
        hostButton.onClick.AddListener(OnClickHost);
        clientButton.onClick.AddListener(OnClickClient);
        shutdownButton.onClick.AddListener(OnClickShutdown);

        nm.OnClientConnectedCallback += HandleClientConnected;
        nm.OnClientDisconnectCallback += HandleClientDisconnected;

        if (lobbyPanel) lobbyPanel.SetActive(true);
        if (shutdownButton) shutdownButton.gameObject.SetActive(false);
        if (statusText) statusText.text = "Listo para conectar.";
    }

    private void OnDisable()
    {
        hostButton.onClick.RemoveListener(OnClickHost);
        clientButton.onClick.RemoveListener(OnClickClient);
        shutdownButton.onClick.RemoveListener(OnClickShutdown);

        if (nm != null)
        {
            nm.OnClientConnectedCallback -= HandleClientConnected;
            nm.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void OnClickHost()
    {
        if (!TryGetAddressAndPort(out string _, out ushort port)) return;

        // El host escucha en todas las interfaces.
        transport.SetConnectionData("0.0.0.0", port, "0.0.0.0");

        if (nm.StartHost())
        {
            if (statusText) statusText.text = $"HOST escuchando en puerto {port}. Esperando jugadores...";
            SetLobbyInteractable(false);
            shutdownButton.gameObject.SetActive(true);
        }
        else
        {
            if (statusText) statusText.text = "Error iniciando Host.";
        }
    }

    private void OnClickClient()
    {
        if (!TryGetAddressAndPort(out string address, out ushort port)) return;

        transport.SetConnectionData(address, port);

        if (nm.StartClient())
        {
            if (statusText) statusText.text = $"CLIENT conectando a {address}:{port}...";
            SetLobbyInteractable(false);
            shutdownButton.gameObject.SetActive(true);
        }
        else
        {
            if (statusText) statusText.text = "Error iniciando Client.";
        }
    }

    private void OnClickShutdown()
    {
        nm.Shutdown();
        if (statusText) statusText.text = "Conexión finalizada.";
        SetLobbyInteractable(true);
        shutdownButton.gameObject.SetActive(false);
    }

    private bool TryGetAddressAndPort(out string address, out ushort port)
    {
        address = (ipInput && !string.IsNullOrWhiteSpace(ipInput.text)) ? ipInput.text.Trim() : "127.0.0.1";
        string portStr = (portInput && !string.IsNullOrWhiteSpace(portInput.text)) ? portInput.text.Trim() : "7777";

        if (!ushort.TryParse(portStr, out port))
        {
            if (statusText) statusText.text = "Puerto inválido. Usa 1–65535.";
            return false;
        }
        return true;
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (nm.IsServer)
        {
            int count = nm.ConnectedClientsIds.Count;
            if (statusText) statusText.text = $"Cliente {clientId} conectado. Jugadores: {count}/{minPlayersToStart}";

            if (count >= minPlayersToStart)
            {
                nm.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
                if (lobbyPanel) lobbyPanel.SetActive(false);
            }
        }
        else
        {
            if (statusText) statusText.text = $"Conectado. ClientID local: {nm.LocalClientId}";
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (statusText) statusText.text = $"Cliente {clientId} desconectado.";
    }

    private void SetLobbyInteractable(bool enabled)
    {
        if (ipInput) ipInput.interactable = enabled;
        if (portInput) portInput.interactable = enabled;
        if (hostButton) hostButton.interactable = enabled;
        if (clientButton) clientButton.interactable = enabled;
    }
}
