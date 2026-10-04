using UnityEngine;

public class EnemyScore : MonoBehaviour
{
    [Header("Score Value")]
    public int scoreValue = 300;

    private bool scoreAdded = false;

    public void GiveScore()
    {
        // ป้องกันการให้คะแนนซ้ำ
        if (scoreAdded)
            return;

        scoreAdded = true;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
        }
    }
}
