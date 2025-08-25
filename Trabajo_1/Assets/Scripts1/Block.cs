using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Identificador del bloque")]
    public ItemID id = ItemID.Cobblestone;   // en tu caso solo usarás piedra

    [HideInInspector] public Slot currentSlot;
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;
    [HideInInspector] public Transform startParent;

    private void Awake()
    {
        // guarda la posición original (por si necesita volver)
        startPos = transform.position;
        startRot = transform.rotation;
        startParent = transform.parent;
    }

    /// <summary>
    /// Devuelve este bloque a su posición inicial.
    /// </summary>
    public void ResetToStart()
    {
        transform.SetPositionAndRotation(startPos, startRot);
        transform.SetParent(startParent, true);

        if (currentSlot != null)
        {
            currentSlot.Clear();
        }
        currentSlot = null;
    }

    /// <summary>
    /// Marca la posición actual como inicio.
    /// </summary>
    public void MarkAsStart()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        startParent = transform.parent;
    }
}
