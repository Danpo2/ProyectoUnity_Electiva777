using UnityEngine;
using System.Linq;

public class Bootstrap : MonoBehaviour
{
    [Header("Opcional: llenar inventario al iniciar")]
    public bool autoFillInventory = true;
    public GameObject cobbleBlockPrefab;   // tu prefab de piedra
    public int stonesToSpawn = 9;          // cuántas piedras iniciales

    [Header("Registrar bloques puestos a mano")]
    public bool autoRegisterExistingBlocks = true;

    void Awake()
    {
        var cm = FindObjectOfType<CraftingManager>();
        if (cm == null)
        {
            Debug.LogError("Bootstrap: No encontré CraftingManager en la escena.");
            return;
        }

        // 1) Enlazar la grilla 3x3 de la mesa por gridIndex (x=columna, y=fila)
        int linked = 0;
        foreach (var s in FindObjectsOfType<Slot>())
        {
            if (s.isCraftingSlot &&
                s.gridIndex.x >= 0 && s.gridIndex.x < 3 &&
                s.gridIndex.y >= 0 && s.gridIndex.y < 3)
            {
                cm.grid[s.gridIndex.x, s.gridIndex.y] = s;
                linked++;
            }
        }
        if (linked != 9)
        {
            Debug.LogWarning($"Bootstrap: Slots de mesa enlazados = {linked}/9. Revisa gridIndex e isCraftingSlot.");
        }

        // 2) Registrar bloques que dejaste a mano dentro de los slots (inventario o mesa)
        if (autoRegisterExistingBlocks)
        {
            foreach (var s in FindObjectsOfType<Slot>())
            {
                // Busca un Block hijo inmediato (por si lo pusiste en el editor)
                var b = s.GetComponentInChildren<Block>();
                if (b != null)
                {
                    // Coloca formalmente y marca origen para ResetToStart()
                    if (s.TryPlace(b))
                        b.MarkAsStart();
                }
            }
        }

        // 3) Llenar inventario con piedras (si hay slots vacíos)
        if (autoFillInventory)
        {
            if (cobbleBlockPrefab == null)
            {
                Debug.LogWarning("Bootstrap: autoFillInventory activo pero 'cobbleBlockPrefab' no asignado.");
            }
            else
            {
                var invSlots = FindObjectsOfType<Slot>()
                    .Where(s => !s.isCraftingSlot)  // solo inventario
                    .ToList();

                int placed = 0;
                foreach (var slot in invSlots)
                {
                    if (placed >= stonesToSpawn) break;
                    if (!slot.IsEmpty()) continue;

                    var spawnPos = slot.snapPoint ? slot.snapPoint.position : slot.transform.position;
                    var spawnRot = slot.snapPoint ? slot.snapPoint.rotation : slot.transform.rotation;

                    var go = Instantiate(cobbleBlockPrefab, spawnPos, spawnRot);
                    var block = go.GetComponent<Block>();
                    if (block == null)
                    {
                        Debug.LogError("Bootstrap: el prefab de piedra no tiene componente Block.");
                        Destroy(go);
                        continue;
                    }

                    if (slot.TryPlace(block))
                    {
                        block.MarkAsStart();
                        placed++;
                    }
                    else
                    {
                        Destroy(go);
                    }
                }
                Debug.Log($"Bootstrap: piedras generadas en inventario = {placed}");
            }
        }

        // 4) Señal: CraftingManager listo
        cm.isReady = true;
        Debug.Log("Bootstrap: CraftingManager listo (isReady = true).");
    }
}
