using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Play);
    }

    void Play()
    {
        Debug.Log("Click SFX, instance = " + (SFXPlayer.I != null));
        SFXPlayer.I?.PlayButton();
    }


}
