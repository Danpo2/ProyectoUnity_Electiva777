using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseInitializer : MonoBehaviour
{
    public UnityEngine.UI.Button[] buttonsToEnable;

    void Awake()
    {
        foreach (var b in buttonsToEnable) if (b) b.interactable = false;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Debug.Log("Firebase listo");
                foreach (var b in buttonsToEnable) if (b) b.interactable = true;
            }
            else
            {
                Debug.LogError($"Firebase dependencias no resueltas: {status}");
            }
        });
    }
}
