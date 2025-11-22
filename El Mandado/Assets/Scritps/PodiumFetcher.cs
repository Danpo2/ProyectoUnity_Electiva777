using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;

public class PodiumFetcher : MonoBehaviour
{
    public PodiumModal podiumModal;

    public IEnumerator FetchPodiumDataCoroutine()
    {
        Debug.Log("[Podium] Empezando fetch...");

        var task = FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .OrderByChild("stats/highScore")   // ordena por highScore
            .LimitToLast(3)
            .GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogError("[Podium] Error en task: " + task.Exception);
            yield break;
        }

        var snap = task.Result;
        Debug.Log("[Podium] Snap children count = " + snap.ChildrenCount);

        var result = new List<PlayerScore>();

        foreach (var child in snap.Children)
        {
            var nameNode = child.Child("profile").Child("name");
            var scoreNode = child.Child("stats").Child("highScore");

            string nombre = (nameNode != null && nameNode.Value != null)
                ? nameNode.Value.ToString()
                : "-";

            int score = 0;
            if (scoreNode != null && scoreNode.Value != null)
                int.TryParse(scoreNode.Value.ToString(), out score);

            result.Add(new PlayerScore(nombre, score));
        }

        // Firebase devuelve ascendente: hacemos descendente
        result.Reverse();

        if (result.Count == 0)
        {
            Debug.LogWarning("[Podium] No hay jugadores en la DB");
            result.Add(new PlayerScore("-", 0));
            result.Add(new PlayerScore("-", 0));
            result.Add(new PlayerScore("-", 0));
        }
        else
        {
            while (result.Count < 3)
                result.Add(new PlayerScore("-", 0));
        }

        podiumModal.ShowPodium(result.ToArray());
    }

}
