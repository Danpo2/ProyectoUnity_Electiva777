using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    // Llama desde OnClick() pasando el índice de escena configurado en Build Settings.
    public void Load(int buildIndex)
    {
        if (buildIndex >= 0 && buildIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(buildIndex);
        else
            Debug.LogWarning("Índice de escena inválido: " + buildIndex);
    }
}
