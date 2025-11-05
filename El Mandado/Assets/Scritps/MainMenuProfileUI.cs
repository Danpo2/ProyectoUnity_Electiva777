using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuProfileUI : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button saveButton;
    public TextMeshProUGUI feedback;

    async void Start()
    {
        while (FirebaseManager.I != null && !FirebaseManager.I.IsReady)
            await System.Threading.Tasks.Task.Yield();
        // podrías precargar name si existe (opcional)
    }

    public async void SaveName()
    {
        var n = nameInput.text.Trim();
        if (string.IsNullOrEmpty(n)) { if (feedback) feedback.text = "Escribe un nombre."; return; }
        await FirebaseManager.I.SavePlayerNameAsync(n);
        if (feedback) feedback.text = "¡Nombre guardado!";
    }
}
