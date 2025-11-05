using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager I;
    public bool IsReady { get; private set; }
    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser User { get; private set; }
    public DatabaseReference DB { get; private set; }

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        _ = InitFirebase();
    }

    async Task InitFirebase()
    {
        var dep = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dep != DependencyStatus.Available)
        {
            Debug.LogError($"[Firebase] Dependencias no disponibles: {dep}");
            return;
        }

        Auth = FirebaseAuth.DefaultInstance;

        // 🔧 Solución del warning:
        var app = FirebaseApp.DefaultInstance;
        app.Options.DatabaseUrl = new System.Uri("https://el-mandado-11eb8-default-rtdb.firebaseio.com/");

        // Ahora sí:
        DB = FirebaseDatabase.GetInstance(app).RootReference;

    }

    string UserPath(string sub = "") =>
        string.IsNullOrEmpty(sub) ? $"users/{User.UserId}" : $"users/{User.UserId}/{sub}";

    public async Task SavePlayerNameAsync(string name) =>
        await DB.Child(UserPath("profile/name")).SetValueAsync(name);

    public async Task SubmitHighScoreAsync(int score)
    {
        var snap = await DB.Child(UserPath("stats/highScore")).GetValueAsync();
        int current = 0; if (snap.Exists && snap.Value != null) int.TryParse(snap.Value.ToString(), out current);
        if (score > current) await DB.Child(UserPath("stats/highScore")).SetValueAsync(score);
    }
}
