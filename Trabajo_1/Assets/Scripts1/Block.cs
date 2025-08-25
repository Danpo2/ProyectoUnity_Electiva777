using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Identificador del bloque")]
    public ItemID id = ItemID.Cobblestone;   // en tu caso solo usar�s piedra

    [HideInInspector] public Slot currentSlot;
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;
    [HideInInspector] public Transform startParent;

    private void Awake()
    {
        // guarda la posici�n original (por si necesita volver)
        startPos = transform.position;
        startRot = transform.rotation;
        startParent = transform.parent;
    }

    /// <summary>
    /// Devuelve este bloque a su posici�n inicial.
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
    /// Marca la posici�n actual como inicio.
    /// </summary>
    public void MarkAsStart()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        startParent = transform.parent;
    }
}
