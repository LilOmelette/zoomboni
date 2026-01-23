using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{

    private int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finishText;
    public TextMeshProUGUI timeText;
    public Timer timerLevelDuration;

    /** SINGLETON **/
    private static LevelManager instance;
    public static LevelManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("Tried to access LevelManager before its instantiation");
        }
        return instance;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        scoreText.text = "Score: " + score;
        finishText.text = "";
        timerLevelDuration.Reset();
    }
    public void AddPoints(int points)
    {
        score += points;
    }

    public void Update()
    {
        if (timerLevelDuration.JustDeactivated())
        {
            scoreText.text = "";
            timeText.text = "";
            finishText.text = "Time's up! Score : " + score;
        }

        else if (timerLevelDuration.IsActive())
        {
            float timeRemaining =
                Mathf.Round(
                    timerLevelDuration.GetMax()
                     * (1.0f - timerLevelDuration.GetPercent())
                    / 60f
                );
            timeRemaining = Mathf.Clamp(timeRemaining, 0, int.MaxValue);

            timeText.text = "TIME: " + timeRemaining;

            scoreText.text = "Score: " + score;
        }
    }

}
