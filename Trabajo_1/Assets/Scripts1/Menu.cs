using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Este método lo vas a llamar desde el botón
    public void LoadGame()
    {
        SceneManager.LoadScene("Game"); // 👈 Nombre exacto de tu escena
    }
}
