using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;

    public int CurrentScore { get; private set; }

    private void Start()
    {
        ResetScore();
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;

        UpdateScoreUI();
    }

    public void ResetScore()
    {
        CurrentScore = 0;

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {CurrentScore}";
        }
    }
}