using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameSpawner : NetworkBehaviour
{
    [Header("Spawn Bounds (centro en 0,0)")]
    [SerializeField] private float halfSize = 50f; // Terreno 100x100 => mitad 50

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Coleccionables")]
    [SerializeField] private int collectiblesCount = 30;

    private readonly Dictionary<ulong, NetworkObject> spawnedPlayers = new();

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
    }

    private void OnServerStarted()
    {
        if (!IsServer) return;

        // Spawnear ya conectados (host por ejemplo)
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            SpawnPlayerFor(clientId);

        // Spawnear coleccionables una sola vez
        SpawnCollectibles();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;
        SpawnPlayerFor(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        if (spawnedPlayers.TryGetValue(clientId, out var netObj) && netObj != null && netObj.IsSpawned)
        {
            netObj.Despawn(true);
        }
        spawnedPlayers.Remove(clientId);
    }

    private void SpawnPlayerFor(ulong clientId)
    {
        if (playerPrefab == null) { Debug.LogError("Falta playerPrefab en GameSpawner"); return; }

        Vector3 pos = GetRandomPointOnMap();
        var go = Instantiate(playerPrefab, pos, Quaternion.identity);
        var netObj = go.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId, true); // asigna ownership al cliente

        spawnedPlayers[clientId] = netObj;
    }

    private void SpawnCollectibles()
    {
        if (collectiblePrefab == null) { Debug.LogWarning("Falta collectiblePrefab, no se generarán coleccionables."); return; }

        for (int i = 0; i < collectiblesCount; i++)
        {
            Vector3 pos = GetRandomPointOnMap();
            var go = Instantiate(collectiblePrefab, pos, Quaternion.identity);
            var no = go.GetComponent<NetworkObject>();
            no.Spawn(true);
        }
    }

    private Vector3 GetRandomPointOnMap()
    {
        float x = Random.Range(-halfSize + 1f, halfSize - 1f);
        float z = Random.Range(-halfSize + 1f, halfSize - 1f);
        return new Vector3(x, 0.05f, z); // un poco sobre el suelo
    }
}
