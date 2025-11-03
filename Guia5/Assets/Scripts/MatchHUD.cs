using UnityEngine;
using UnityEngine.UI;

public class MatchHUD : MonoBehaviour
{
    [SerializeField] private Text endText;

    public void ShowEnd(string msg)
    {
        if (!endText) return;
        endText.text = msg;
        endText.gameObject.SetActive(true);
    }
}

