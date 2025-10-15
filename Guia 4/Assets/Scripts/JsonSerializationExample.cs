using UnityEngine;

public class JsonSerializationExample : MonoBehaviour
{
    void Start()
    {
        var basicObject = new BasicObject
        {
            shield = 100,
            health = 50,
            name = "Sven The Explorer",
            position = new Vector3(1, 2, 3)
        };

        string json = JsonUtility.ToJson(basicObject);
        Debug.Log(json);

        BasicObject copy = JsonUtility.FromJson<BasicObject>(json);
        Vector3 pos = copy.position;
        Debug.Log($"{copy.name} at {pos.x}, {pos.y}, {pos.z}");
    }
}
