using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop3D : MonoBehaviour
{
    [Header("Configuraci�n")]
    public Camera cam;               // arrastra tu Main Camera aqu�
    public LayerMask blockMask;      // capa de los bloques (ej: "Block")
    public LayerMask slotMask;       // capa de los slots (ej: "Slot")
    public float planeHeight = 0f;   // altura Y del plano (donde est�n tus Slots y el Quad)
    public float snapRadius = 0.3f;  // radio para buscar slot al soltar

    private Plane dragPlane;
    private Block grabbed;
    private Vector3 grabOffset;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        dragPlane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0));
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0)) BeginAt(Input.mousePosition);
        else if (Input.GetMouseButton(0)) MoveAt(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0)) EndAt(Input.mousePosition);
#else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) BeginAt(t.position);
            else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary) MoveAt(t.position);
            else if (t.phase == TouchPhase.Ended) EndAt(t.position);
        }
#endif
    }

    void BeginAt(Vector3 screenPos)
    {
        if (grabbed != null) return;

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log("Raycast golpe�: " + hit.collider.name);
        }

        if (Physics.Raycast(ray, out hit, 100f, blockMask))
        {
            Debug.Log("Raycast encontr� bloque: " + hit.collider.name);
            grabbed = hit.collider.GetComponent<Block>();
            if (grabbed == null) return;

            // calcular offset
            if (dragPlane.Raycast(ray, out float dist))
            {
                Vector3 worldPoint = ray.GetPoint(dist);
                grabOffset = grabbed.transform.position - worldPoint;
            }

            if (grabbed.currentSlot != null)
            {
                grabbed.currentSlot.Clear();
            }

            grabbed.transform.SetParent(null);
        }
    }

    void MoveAt(Vector3 screenPos)
    {
        if (grabbed == null) return;

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (dragPlane.Raycast(ray, out float dist))
        {
            Vector3 worldPoint = ray.GetPoint(dist);
            Vector3 target = worldPoint + grabOffset;
            target.y = planeHeight; // mantener la altura fija

            grabbed.transform.position = target;
        }
    }

    void EndAt(Vector3 screenPos)
    {
        if (grabbed == null) return;

        // busca slots cercanos
        Collider[] hits = Physics.OverlapSphere(grabbed.transform.position, snapRadius, slotMask);
        Slot best = null;
        float bestDist = float.MaxValue;

        foreach (var col in hits)
        {
            Slot s = col.GetComponent<Slot>();
            if (s != null && s.IsEmpty())
            {
                float d = (s.transform.position - grabbed.transform.position).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    best = s;
                }
            }
        }

        if (best != null && best.TryPlace(grabbed))
        {
            // colocado correctamente
        }
        else
        {
            // volver al inventario si no encaj�
            grabbed.ResetToStart();
        }

        grabbed = null;

        // opcional: avisar al CraftingManager para comprobar receta
        if (CraftingManager.Instance != null)
        {
            CraftingManager.Instance.Evaluate();
        }
    }
}
