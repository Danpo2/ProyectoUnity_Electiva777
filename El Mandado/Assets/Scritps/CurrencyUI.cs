using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    const string KEY_COINS = "coins";

    void OnEnable() => Refresh();
    public void Refresh()
    {
        int coins = PlayerPrefs.GetInt(KEY_COINS, 0);
        if (coinText) coinText.text = coins.ToString();
    }

    // Útil para pruebas: asigna monedas desde el editor con un botón
    [ContextMenu("Add 100 Coins (Test)")]
    void AddCoinsTest()
    {
        int coins = PlayerPrefs.GetInt(KEY_COINS, 0) + 100;
        PlayerPrefs.SetInt(KEY_COINS, coins);
        Refresh();
    }
}
