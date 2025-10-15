using System;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class RealtimeDatabaseService : MonoBehaviour
{
    DatabaseReference _root;

    void Awake()
    {
        _root = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public Task SavePlayerAsync(Player p)
    {
        string json = JsonUtility.ToJson(p, true);
        return _root.Child("players").Child(p.uid).SetRawJsonValueAsync(json);
    }

    public Task<Player> LoadPlayerAsync(string uid)
    {
        var tcs = new TaskCompletionSource<Player>();
        _root.Child("players").Child(uid).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || !task.IsCompleted)
            {
                tcs.SetException(task.Exception ?? new Exception("LoadPlayerAsync faulted"));
                return;
            }
            var snap = task.Result;
            if (snap.Exists)
            {
                var json = snap.GetRawJsonValue();
                var player = JsonUtility.FromJson<Player>(json);
                tcs.SetResult(player);
            }
            else
            {
                tcs.SetResult(null);
            }
        });
        return tcs.Task;
    }
}
