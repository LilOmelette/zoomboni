using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finishText;
    public TextMeshProUGUI timeText;
    public Timer timerLevelDuration;
    public AudioSource sfxClean;

    public PlayerInput playerInput;

    private int points = 0;

    private InputAction
            inputReset,
            inputEscape;

    private float MAX_POINTS = 0;
    private bool ended = false;

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

        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        finishText.text = "";
        timerLevelDuration.Reset();
        inputReset = playerInput.actions.FindAction("Reset");
        inputEscape = playerInput.actions.FindAction("Escape");

        MAX_POINTS = 0;
        Collectable[] collectables = FindObjectsByType<Collectable>(FindObjectsSortMode.None);
        foreach(Collectable collectable in collectables)
        {
            MAX_POINTS += collectable.GetScore();
        }
        ended = false;
    }
    public void AddPoints(int points)
    {
        this.points += points;
        sfxClean.Play();
    }

    public void Update()
    {
        UpdateInputs();
        UpdateUI();
    }

    private void UpdateInputs()
    {
        if (inputReset.WasPressedThisFrame())
        {
            print("Resetting scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (inputEscape.WasPressedThisFrame())
        {
            print("So long, fair well");
            Application.Quit();
        }
    }

    private void UpdateUI()
    {
        if (points == MAX_POINTS)
        {
            End(true);
        }
        else if (timerLevelDuration.JustDeactivated())
        {
            End(false);
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

            if (timerLevelDuration.GetPercent() >= 0.8f)
            {
                timeText.color = Color.red;
            }

            timeText.text = "Time: " + timeRemaining;

            scoreText.text = "Points: " + points + "/" + MAX_POINTS;
        }
    }

    private void End(bool success)
    {
        if (ended) return;

        ended = true;
        scoreText.text = "";
        timeText.text = "";
        float TIME_N = timerLevelDuration.GetPercent() * timerLevelDuration.GetMax() / 60.0f;
        TIME_N = Mathf.Round(TIME_N * 100.0f) / 100.0f;

        if (success)
        {
            timerLevelDuration.End();
            finishText.text = "Wow! You cleaned up in " + TIME_N + " seconds!\n Press R to reset!";

            float TIME_1 = GetTime("1");
            float TIME_2 = GetTime("2");
            float TIME_3 = GetTime("3");

            if (TIME_N < TIME_1)
            {
                TIME_3 = TIME_2;
                TIME_2 = TIME_1;
                TIME_1 = TIME_N;
            }
            else if (TIME_N < TIME_2)
            {
                TIME_3 = TIME_2;
                TIME_2 = TIME_N;
            }
            else if (TIME_N < TIME_3)
            {
                TIME_3 = TIME_N;
            }

            Debug.Log("Best Times: 1: " + TIME_1 + ", 2: " + TIME_2 + ", 3: " + TIME_3);
            PlayerPrefs.SetFloat("TIME_1", TIME_1 == float.MaxValue ? 0 : TIME_1);
            PlayerPrefs.SetFloat("TIME_2", TIME_2 == float.MaxValue ? 0 : TIME_2);
            PlayerPrefs.SetFloat("TIME_3", TIME_3 == float.MaxValue ? 0 : TIME_3);
            PlayerPrefs.Save();
        }

        else
        {

            finishText.text = "Time out...\n Press R to reset!";
        }
        
    }

    private float GetTime(string n)
    {
        float time = PlayerPrefs.GetFloat("TIME_" + n);
        if (time == 0) return float.MaxValue;
        else return time;
    }

}
