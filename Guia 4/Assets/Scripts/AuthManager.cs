using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    [Header("Refs")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nicknameInputOptional;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI consoleText;

    public RealtimeDatabaseService db;

    FirebaseAuth _auth;
    FirebaseUser _user;

    void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += OnAuthStateChanged;
        OnAuthStateChanged(this, null);
        Log("Auth listo");
    }

    void OnDestroy()
    {
        if (_auth != null) _auth.StateChanged -= OnAuthStateChanged;
    }

    void OnAuthStateChanged(object sender, System.EventArgs e)
    {
        if (_auth.CurrentUser != _user)
        {
            bool signedIn = _auth.CurrentUser != null;
            _user = _auth.CurrentUser;
            statusText.text = signedIn ? $"UID: {_user.UserId}" : "No autenticado";
        }
    }

    public void OnRegisterClicked()
    {
        _ = RegisterAsync(emailInput.text, passwordInput.text);
    }

    public void OnLoginClicked()
    {
        _ = LoginAsync(emailInput.text, passwordInput.text);
    }

    public void OnLogoutClicked()
    {
        _auth.SignOut();
        Log("Sesión cerrada");

        // 🔹 Limpia todos los input fields
        if (emailInput) emailInput.text = "";
        if (passwordInput) passwordInput.text = "";
        if (nicknameInputOptional) nicknameInputOptional.text = "";

        // 🔹 Limpia textos de estado y consola
        if (statusText) statusText.text = "No autenticado";
        if (consoleText) consoleText.text = "";

        // 🔹 Si quieres también resetear el jugador en memoria
        _user = null;
    }


    async Task RegisterAsync(string email, string password)
    {
        try
        {
            var result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            _user = result.User;
            Log($"Registrado: {_user.Email} ({_user.UserId})");

            // Crear perfil inicial en DB
            var nickname = string.IsNullOrWhiteSpace(nicknameInputOptional?.text) ? "Player" : nicknameInputOptional.text;
            var player = new Player(_user.UserId, nickname);
            await db.SavePlayerAsync(player);

            // Cargar y mostrar JSON
            var loaded = await db.LoadPlayerAsync(_user.UserId);
            ShowPlayerJson(loaded);
            Log("✅ Registro completado correctamente.");
            statusText.text = $"Usuario registrado: {_user.Email}";

        }
        catch (System.Exception ex)
        {
            // Firebase lanza Firebase.FirebaseException con un ErrorCode (int)
            var fex = ex as Firebase.FirebaseException;
            int code = fex != null ? fex.ErrorCode : -1;

            // Intenta mapear a AuthError para ver el nombre simbólico
            string codeName;
            try { codeName = ((Firebase.Auth.AuthError)code).ToString(); }
            catch { codeName = "Unknown"; }

            LogError($"[Auth FAIL] {codeName} ({code}) :: {ex.Message}\n{ex}");

            // Confirma de nuevo opciones (ya vimos que están SET, pero lo dejamos)
            var app = Firebase.FirebaseApp.DefaultInstance;
            if (app != null)
            {
                var o = app.Options;
                Log($"[AppOpts] ProjectId={o.ProjectId} AppId={o.AppId} ApiKey={(string.IsNullOrEmpty(o.ApiKey) ? "EMPTY" : "SET")}");
            }
        }



    }

    async Task LoginAsync(string email, string password)
    {
        try
        {
            var result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
            _user = result.User;
            Log($"Login OK: {_user.Email} ({_user.UserId})");

            // Cargar jugador y mostrar JSON
            var loaded = await db.LoadPlayerAsync(_user.UserId);
            if (loaded == null)
            {
                Log("No había perfil; creando uno por defecto.");
                var player = new Player(_user.UserId, "Player");
                await db.SavePlayerAsync(player);
                loaded = player;
                Log("✅ Inicio de sesión exitoso.");
                statusText.text = $"Sesión iniciada: {_user.Email}";

            }
            ShowPlayerJson(loaded);
        }
        catch (System.Exception ex)
        {
            // Firebase lanza Firebase.FirebaseException con un ErrorCode (int)
            var fex = ex as Firebase.FirebaseException;
            int code = fex != null ? fex.ErrorCode : -1;

            // Intenta mapear a AuthError para ver el nombre simbólico
            string codeName;
            try { codeName = ((Firebase.Auth.AuthError)code).ToString(); }
            catch { codeName = "Unknown"; }

            LogError($"[Auth FAIL] {codeName} ({code}) :: {ex.Message}\n{ex}");

            // Confirma de nuevo opciones (ya vimos que están SET, pero lo dejamos)
            var app = Firebase.FirebaseApp.DefaultInstance;
            if (app != null)
            {
                var o = app.Options;
                Log($"[AppOpts] ProjectId={o.ProjectId} AppId={o.AppId} ApiKey={(string.IsNullOrEmpty(o.ApiKey) ? "EMPTY" : "SET")}");
            }
        }



    }

    void ShowPlayerJson(Player p)
    {
        var json = JsonUtility.ToJson(p, true);
        statusText.text = $"Jugador: {p.nickname} (lvl {p.level})";
        Log("JSON Player:\n" + json);
    }

    void Log(string msg)
    {
        Debug.Log(msg);
        if (consoleText) consoleText.text = msg + "\n\n" + consoleText.text;
    }

    void LogError(string msg)
    {
        Debug.LogError(msg);
        if (consoleText) consoleText.text = "[ERROR] " + msg + "\n\n" + consoleText.text;
    }

}
