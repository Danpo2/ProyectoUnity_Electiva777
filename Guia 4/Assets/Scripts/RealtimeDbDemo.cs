using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;

public class RealtimeDbDemo : MonoBehaviour
{
    private DatabaseReference _root;

    void Start()
    {
        // Habilitar cache offline antes de cualquier operación
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);

        // Caso A: DB por defecto del proyecto
        _root = FirebaseDatabase.DefaultInstance.RootReference;

        // Caso B: Si tu DB requiere URL explícita (región)
        // var db = FirebaseDatabase.GetInstance("https://tu-proyecto-default-rtdb.<region>.firebasedatabase.app/");
        // _root = db.RootReference;

        // Escribir & Leer una vez
        _ = EscribirYLeer();

        // Suscribirse a cambios en tiempo real
        FirebaseDatabase.DefaultInstance
            .GetReference("demo/nombre")
            .ValueChanged += OnNombreChanged;
    }

    async Task EscribirYLeer()
    {
        // Create/Update
        await _root.Child("demo").Child("nombre").SetValueAsync("Hola Firebase desde Unity");

        // Read once
        var snap = await _root.Child("demo/nombre").GetValueAsync();
        if (snap.Exists)
        {
            Debug.Log("Valor leído: " + snap.Value);
        }
        else
        {
            Debug.Log("No existe 'demo/nombre'");
        }
    }

    void OnNombreChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.DatabaseError != null)
        {
            Debug.LogError(e.DatabaseError.Message);
            return;
        }

        Debug.Log("Cambio detectado: " + e.Snapshot.Value);
    }
}