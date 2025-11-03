using Unity.Netcode;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<int> Score = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Solo el servidor modifica el marcador para evitar trampas
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void AddScoreServerRpc(int delta)
    {
        Score.Value += delta;
    }
}
