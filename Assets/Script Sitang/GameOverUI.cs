using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Game Over UI")]
    public TextMeshProUGUI scoreText;

    void Start()
    {
        if (scoreText == null)
            return;

        if (ScoreManager.instance != null)
        {
            int finalScore =
                ScoreManager.instance.GetScore();

            scoreText.text =
                "Score: " +
                finalScore.ToString("N0");
        }
        else
        {
            scoreText.text = "Score: 0";
        }
    }
}