using UnityEngine;
using System;

public class WristCurlsGameController : MonoBehaviour
{
    private float[] highScores = new float[3];
    private float gameTime = 0f;

    [Header("UI Settings")]
    public GUIStyle timerStyle;
    public GUIStyle highScoreStyle;

    public float CurrentGameTime => gameTime;  // Expose current game time

    void Start()
    {
        LoadHighScores();

        // Initialize UI styles if not assigned
        if (timerStyle == null)
        {
            timerStyle = new GUIStyle
            {
                fontSize = 40,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
        }
        if (highScoreStyle == null)
        {
            highScoreStyle = new GUIStyle
            {
                fontSize = 40,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseManager.Instance.TogglePause();
        }

        // Increment timer only when not paused
        if (!PauseManager.Instance.IsPaused)
        {
            gameTime += Time.deltaTime;
        }
    }

    void OnGUI()
    {
        GUI.color = Color.white;
        GUI.Label(new Rect(20, 15, 200, 40), $"Time: {gameTime:F2}", timerStyle);
        GUI.Label(new Rect(20, 60, 200, 30), "High Scores:", highScoreStyle);
        for (int i = 0; i < highScores.Length; i++)
        {
            if (highScores[i] > 0)
            {
                GUI.Label(new Rect(20, 85 + (i * 20), 200, 30),
                          $"{i + 1}. {highScores[i]:F2}",
                          highScoreStyle);
            }
        }
    }

    void LoadHighScores()
    {
        for (int i = 0; i < highScores.Length; i++)
        {
            highScores[i] = PlayerPrefs.GetFloat($"HighScore{i}", 0f);
        }
    }

    void SaveHighScore(float newScore)
    {
        float[] newScores = new float[highScores.Length + 1];
        Array.Copy(highScores, newScores, highScores.Length);
        newScores[highScores.Length] = newScore;
        Array.Sort(newScores);
        Array.Reverse(newScores);
        Array.Copy(newScores, highScores, highScores.Length);

        for (int i = 0; i < highScores.Length; i++)
        {
            PlayerPrefs.SetFloat($"HighScore{i}", highScores[i]);
        }
        PlayerPrefs.Save();
    }

    public void ResetGame()
    {
        SaveHighScore(gameTime);
        gameTime = 0f;
        // Despawn all spikes when the player dies
        foreach (GameObject spike in GameObject.FindGameObjectsWithTag("Spike"))
        {
            Destroy(spike);
        }
    }
}
