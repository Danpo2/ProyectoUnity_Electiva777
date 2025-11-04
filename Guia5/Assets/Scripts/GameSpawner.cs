using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameSpawner : MonoBehaviour   // ← antes: NetworkBehaviour
{
    [Header("Spawn Bounds (centro en 0,0)")]
    [SerializeField] private float halfSize = 50f;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Coleccionables")]
    [SerializeField] private int collectiblesCount = 30;

    private readonly Dictionary<ulong, NetworkObject> spawnedPlayers = new();
    private bool didInitialSpawn;

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void Start()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        if (nm.IsServer && nm.IsListening && !didInitialSpawn)
        {
            Debug.Log("[Spawner] Start() en GameScene -> spawn inicial");
            foreach (var id in nm.ConnectedClientsIds)
                SpawnPlayerFor(id);

            SpawnCollectibles();
            didInitialSpawn = true;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        var nm = NetworkManager.Singleton;
        if (nm == null || !nm.IsServer) return;

        Debug.Log($"[Spawner] OnClientConnected -> {clientId}");
        SpawnPlayerFor(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        var nm = NetworkManager.Singleton;
        if (nm == null || !nm.IsServer) return;

        if (spawnedPlayers.TryGetValue(clientId, out var netObj) && netObj && netObj.IsSpawned)
            netObj.Despawn(true);

        spawnedPlayers.Remove(clientId);
    }

    private void SpawnPlayerFor(ulong clientId)
    {
        if (!playerPrefab) { Debug.LogError("Falta playerPrefab en GameSpawner"); return; }

        Vector3 pos = GetRandomPointOnMap();
        var go = Instantiate(playerPrefab, pos, Quaternion.identity);
        var netObj = go.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId, true);

        spawnedPlayers[clientId] = netObj;
        Debug.Log($"[Spawner] Player spawneado para {clientId} en {pos}");
    }

    private void SpawnCollectibles()
    {
        if (!collectiblePrefab)
        { Debug.LogWarning("Falta collectiblePrefab, no se generarán coleccionables."); return; }

        Debug.Log($"[Spawner] Spawning {collectiblesCount} collectibles");
        for (int i = 0; i < collectiblesCount; i++)
        {
            Vector3 pos = GetRandomPointOnMap();
            var go = Instantiate(collectiblePrefab, pos, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn(true);
        }
    }

    private Vector3 GetRandomPointOnMap()
    {
        float x = Random.Range(-halfSize + 1f, halfSize - 1f);
        float z = Random.Range(-halfSize + 1f, halfSize - 1f);
        return new Vector3(x, 0.05f, z);
    }
}
