using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MainMenuProfileUI : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button saveButton;
    public TextMeshProUGUI feedback;

    async void Start()
    {
        while (FirebaseManager.I == null)
            await Task.Yield();

        Debug.Log("[UI] Encontró FirebaseManager, esperando IsReady...");

        while (!FirebaseManager.I.IsReady)
            await Task.Yield();

        Debug.Log("[UI] Firebase IsReady = true");
    }


    public async void SaveName()
    {
        if (FirebaseManager.I == null || !FirebaseManager.I.IsReady)
        {
            if (feedback) feedback.text = "Firebase no está listo.";
            return;
        }

        var n = nameInput.text.Trim();
        if (string.IsNullOrEmpty(n))
        {
            if (feedback) feedback.text = "Escribe un nombre.";
            return;
        }

        await FirebaseManager.I.SavePlayerNameAsync(n);
        if (feedback) feedback.text = "¡Nombre guardado!";

        if (saveButton) saveButton.gameObject.SetActive(false); // Oculta el botón
    }

}
