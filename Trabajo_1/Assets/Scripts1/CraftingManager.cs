using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    [Header("Grilla 3x3 de la mesa (x=columna, y=fila)")]
    public Slot[,] grid = new Slot[3, 3];

    [Header("Resultado")]
    public GameObject furnacePrefab;        // Prefab del horno
    public Transform resultSpawn;           // Dónde aparece el horno

    [Header("UI de victoria")]
    public WinUIController winUI;           // 👈 arrastra aquí tu UIManager en el Inspector

    [HideInInspector] public bool isReady = false;

    void Awake()
    {
        Instance = this;
    }

    /// Llama esto al soltar un bloque en cualquier slot.
    public void Evaluate()
    {
        if (!isReady) return;

        if (MatchesFurnaceRecipe())
        {
            Craft(ItemID.Furnace);
            ClearGrid();
        }
    }

    /// Receta del horno: anillo de piedra (todo menos el centro).
    bool MatchesFurnaceRecipe()
    {
        // Seguridad: toda la grilla debe estar asignada
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
            {
                if (grid[x, y] == null)
                {
                    Debug.LogWarning($"Crafting grid sin asignar en ({x},{y}).");
                    return false;
                }
            }

        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
            {
                var slot = grid[x, y];
                var block = slot.currentBlock;

                // Centro (1,1) vacío
                if (x == 1 && y == 1)
                {
                    if (block != null) return false;
                }
                else
                {
                    // El resto piedra
                    if (block == null || block.id != ItemID.Cobblestone) return false;
                }
            }

        return true;
    }

    void Craft(ItemID result)
    {
        if (result == ItemID.Furnace)
        {
            if (furnacePrefab == null)
            {
                Debug.LogError("Falta asignar 'furnacePrefab' en CraftingManager.");
                return;
            }

            Vector3 pos = resultSpawn ? resultSpawn.position : Vector3.zero;
            Quaternion rot = resultSpawn ? resultSpawn.rotation : Quaternion.identity;
            Instantiate(furnacePrefab, pos, rot);
            Debug.Log("¡Horno crafteado!");

            // 👇 Mostrar panel de victoria SOLO cuando se hace el horno
            if (winUI != null) winUI.ShowWin();
        }
    }

    /// Limpia los 9 slots de la mesa
    void ClearGrid()
    {
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
            {
                var slot = grid[x, y];
                if (slot != null && slot.currentBlock != null)
                {
                    Destroy(slot.currentBlock.gameObject);
                    slot.Clear();
                }
            }
    }
}
