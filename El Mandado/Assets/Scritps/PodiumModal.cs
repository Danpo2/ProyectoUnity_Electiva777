using UnityEngine;
using TMPro;

public class PodiumModal : ModalView
{
    [Header("Podium UI")]
    public TextMeshProUGUI firstName;
    public TextMeshProUGUI firstScore;
    public TextMeshProUGUI secondName;
    public TextMeshProUGUI secondScore;
    public TextMeshProUGUI thirdName;
    public TextMeshProUGUI thirdScore;

    // Oculta el panel en Awake, solo se muestra con Open()
    void Awake()
    {
        gameObject.SetActive(false); // oculto al inicio
    }


    // Actualiza textos y muestra el panel animado
    public void ShowPodium(PlayerScore[] top3)
    {
        firstName.text = top3[0].playerName;
        firstScore.text = top3[0].score.ToString();
        secondName.text = top3[1].playerName;
        secondScore.text = top3[1].score.ToString();
        thirdName.text = top3[2].playerName;
        thirdScore.text = top3[2].score.ToString();

        gameObject.SetActive(true);
        Open();
    }


}

// Estructura de puntaje
public struct PlayerScore
{
    public string playerName;
    public int score;

    public PlayerScore(string name, int score)
    {
        playerName = name;
        this.score = score;
    }
}
