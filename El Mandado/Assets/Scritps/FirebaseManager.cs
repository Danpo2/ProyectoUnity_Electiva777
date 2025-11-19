using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager I;

    public bool IsReady { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser User { get; private set; }
    public DatabaseReference DB { get; private set; }

    // Si NO quieres Auth todavía, usa esto como ID de jugador.
    public string PlayerId { get; private set; }

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
        _ = InitFirebase();
    }

    async Task InitFirebase()
    {
        Debug.Log("[Firebase] Iniciando...");

        var dep = await FirebaseApp.CheckAndFixDependenciesAsync();
        Debug.Log("[Firebase] Resultado dependencias: " + dep);

        if (dep != DependencyStatus.Available)
        {
            Debug.LogError($"[Firebase] Dependencias no disponibles: {dep}");
            IsReady = false;
            return;
        }

        var app = FirebaseApp.DefaultInstance;

        try
        {
            // Crea la instancia de Database usando directamente la URL
            var db = FirebaseDatabase.GetInstance("https://el-mandado-11eb8-default-rtdb.firebaseio.com/");
            DB = db.RootReference;
            Debug.Log("[Firebase] DB referencia creada con URL directa");
        }
        catch (Exception e)
        {
            Debug.LogError("[Firebase] Error al configurar DatabaseUrl: " + e);
            IsReady = false;
            return;
        }


        Auth = FirebaseAuth.DefaultInstance;
        PlayerId = SystemInfo.deviceUniqueIdentifier;

        IsReady = true;
        Debug.Log("[Firebase] Listo = true");
    }


    // ---------- Helpers de ruta ----------

    // Si USAS Auth (ya tienes User):
    string UserPath(string sub = "")
    {
        // Si aún no hay usuario logueado, fallback al PlayerId
        var id = User != null ? User.UserId : PlayerId;
        return string.IsNullOrEmpty(sub) ? $"users/{id}" : $"users/{id}/{sub}";
    }

    // Guarda solo el nombre del jugador
    public async Task SavePlayerNameAsync(string name)
    {
        if (!IsReady || DB == null)
        {
            Debug.LogWarning("[Firebase] No listo al guardar nombre");
            return;
        }

        await DB.Child(UserPath("profile/name")).SetValueAsync(name);
    }

    // Guarda el récord si el nuevo score es mayor
    public async Task SubmitHighScoreAsync(int score)
    {
        if (!IsReady || DB == null)
        {
            Debug.LogWarning("[Firebase] No listo al guardar highScore");
            return;
        }

        var snap = await DB.Child(UserPath("stats/highScore")).GetValueAsync();

        int current = 0;
        if (snap.Exists && snap.Value != null)
            int.TryParse(snap.Value.ToString(), out current);

        if (score > current)
        {
            await DB.Child(UserPath("stats/highScore")).SetValueAsync(score);
            Debug.Log($"[Firebase] Nuevo récord guardado: {score}");
        }
        else
        {
            Debug.Log($"[Firebase] No es récord. Actual: {current}, nuevo: {score}");
        }
    }
}
