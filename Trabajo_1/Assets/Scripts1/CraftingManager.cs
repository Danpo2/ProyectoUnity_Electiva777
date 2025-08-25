using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    [Header("Grilla 3x3 de la mesa (x=columna, y=fila)")]
    public Slot[,] grid = new Slot[3, 3];   // La llena el Bootstrap por gridIndex (x,y)

    [Header("Resultado")]
    public GameObject furnacePrefab;        // Prefab del horno a instanciar
    public Transform resultSpawn;           // Dónde aparece el horno

    [HideInInspector] public bool isReady = false; // Se pone true cuando grid está enlazada

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Llama esto al soltar un bloque en cualquier slot.
    /// </summary>
    public void Evaluate()
    {
        if (!isReady) return; // Evita nulls si aún no enlazaste la grilla

        if (MatchesFurnaceRecipe())
        {
            Craft(ItemID.Furnace);
            ClearGrid();
        }
    }

    /// <summary>
    /// Receta del horno: anillo de piedra (todo menos el centro).
    /// </summary>
    bool MatchesFurnaceRecipe()
    {
        // Validación de seguridad: toda la grilla debe estar asignada
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

                // Centro (1,1) debe estar vacío
                if (x == 1 && y == 1)
                {
                    if (block != null) return false;
                }
                else
                {
                    // El resto debe ser piedra
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
            Instantiate(furnacePrefab, resultSpawn.position, resultSpawn.rotation);
            Debug.Log("¡Horno crafteado!");
        }
    }

    /// <summary>
    /// Limpia los 9 slots de la mesa (destruye los bloques colocados).
    /// </summary>
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
