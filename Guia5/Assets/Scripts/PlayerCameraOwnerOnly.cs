using UnityEngine;
using Unity.Netcode;

public class PlayerCameraOwnerOnly : NetworkBehaviour
{
    [SerializeField] private Camera cam;

    public override void OnNetworkSpawn()
    {
        if (cam) cam.enabled = IsOwner;
        var au = GetComponentInChildren<AudioListener>(true);
        if (au) au.enabled = IsOwner;
    }
}
