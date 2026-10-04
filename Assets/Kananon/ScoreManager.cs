using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("UI Settings")]
    public TextMeshProUGUI scoreText;

    private int currentScore = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // ให้ ScoreManager อยู่ต่อเมื่อเปลี่ยน Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    // ใช้ดึงคะแนนไปแสดงหน้า Game Over
    public int GetScore()
    {
        return currentScore;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Score: " + currentScore.ToString("N0");
        }
    }
}