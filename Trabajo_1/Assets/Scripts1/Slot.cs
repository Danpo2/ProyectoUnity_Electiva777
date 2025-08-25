using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [Header("Configuración")]
    public bool isCraftingSlot = false;           // true = mesa de crafteo, false = inventario
    public Vector2Int gridIndex = new Vector2Int(-1, -1); // posición en la grilla (solo para mesa)
    public Transform snapPoint;                   // punto donde el bloque se “pega” al entrar

    [Header("Estado")]
    public Block currentBlock;                    // referencia al bloque que está dentro

    /// <summary>
    /// ¿Está vacío este slot?
    /// </summary>
    public bool IsEmpty()
    {
        return currentBlock == null;
    }

    /// <summary>
    /// Intenta poner un bloque en el slot.
    /// Devuelve true si se pudo colocar.
    /// </summary>
    public bool TryPlace(Block b)
    {
        if (!IsEmpty()) return false; // ya ocupado

        currentBlock = b;
        b.currentSlot = this;

        // lo “pega” en el SnapPoint si existe, si no en el slot
        if (snapPoint != null)
        {
            b.transform.position = snapPoint.position;
            b.transform.rotation = snapPoint.rotation;
        }
        else
        {
            b.transform.position = transform.position;
            b.transform.rotation = transform.rotation;
        }

        // lo hace hijo del slot (para que se mueva junto con él)
        b.transform.SetParent(transform, true);

        return true;
    }

    /// <summary>
    /// Limpia el slot y suelta el bloque que tenía.
    /// </summary>
    public void Clear()
    {
        if (currentBlock != null)
        {
            currentBlock.currentSlot = null;
            currentBlock = null;
        }
    }
}

