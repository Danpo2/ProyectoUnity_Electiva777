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

        while (!FirebaseManager.I.IsReady)
            await Task.Yield();

        // Siempre que arranca escena principal en esta ejecución,
        // si no hay nombre en memoria, mostramos el input vacío.
        if (string.IsNullOrEmpty(FirebaseManager.I.PlayerName))
        {
            nameInput.text = "";
            if (saveButton) saveButton.gameObject.SetActive(true);
        }
        else
        {
            // Si ya puso nombre en esta ejecución, lo mostramos
            nameInput.text = FirebaseManager.I.PlayerName;
            if (saveButton) saveButton.gameObject.SetActive(false);
        }
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

        // Guarda en memoria para esta ejecución
        FirebaseManager.I.PlayerName = n;

        // Y también en Firebase (persistente)
        await FirebaseManager.I.SavePlayerNameAsync(n);

        if (feedback) feedback.text = "¡Nombre guardado!";
        if (saveButton) saveButton.gameObject.SetActive(false);
    }
}

