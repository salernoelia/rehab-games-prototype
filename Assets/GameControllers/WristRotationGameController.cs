using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class WristRotationGameController : MonoBehaviour
{
    private float[] highScores = new float[3];

    [Header("References")]
    public Transform floor;
    public Transform circle;

    [Header("Floor Settings")]
    public float rotationSensitivity = 5f;
    private const float NUDGE_STRENGTH = 0.5f;
    public Vector2 initialFloorScale = new Vector2(10f, 10f);
    public float shrinkRate = 0.1f;
    public float minFloorWidth = 3f;

    [Header("UI Settings")]
    public GUIStyle timerStyle;
    public GUIStyle highScoreStyle;

    [Header("Circle Settings")]
    public float deathY = -10f;

    private Vector2 currentFloorScale;
    private float gameTime;
    private Vector3 circleStartPos;

    void Start()
    {
        Time.timeScale = 1; // Ensure game runs after returning
        gameTime = 0f;
        currentFloorScale = initialFloorScale;
        floor.localScale = new Vector3(currentFloorScale.x, currentFloorScale.y, 1);

        if (circle != null)
        {
            circleStartPos = circle.position;
        }

        LoadHighScores();

        if (timerStyle == null)
        {
            timerStyle = new GUIStyle();
            timerStyle.fontSize = 80;
            timerStyle.fontStyle = FontStyle.Bold;
            timerStyle.normal.textColor = Color.white;
        }

        if (highScoreStyle == null)
        {
            highScoreStyle = new GUIStyle();
            highScoreStyle.fontSize = 80;
            highScoreStyle.fontStyle = FontStyle.Bold;
            highScoreStyle.normal.textColor = Color.white;
        }
    }

    void Update()
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused)
            return;
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;

        if (floor == null) return;

        HandleFloorRotation();
        ShrinkFloor();
        CheckCirclePosition();

    }

    void LoadHighScores()
    {
        for (int i = 0; i < 3; i++)
        {
            highScores[i] = PlayerPrefs.GetFloat($"HighScore{i}", 0f);
        }
    }

    void SaveHighScore(float newScore)
    {
        float[] newScores = new float[4];
        Array.Copy(highScores, newScores, 3);
        newScores[3] = newScore;
        Array.Sort(newScores);
        Array.Reverse(newScores);
        Array.Copy(newScores, highScores, 3);

        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetFloat($"HighScore{i}", highScores[i]);
        }
        PlayerPrefs.Save();
    }

    void HandleFloorRotation()
    {
        if (floor != null && OscReceiver.Instance != null)
        {
            float gyroZ = OscReceiver.Instance.gyroZ;
            float rotationDelta = gyroZ * rotationSensitivity * Time.deltaTime;
            float currentAngle = floor.rotation.eulerAngles.z;
            if (currentAngle > 180) currentAngle -= 360;
            float newAngle = currentAngle + rotationDelta;
            if (Mathf.Abs(newAngle) < 2f)
            {
                float nudge = (newAngle >= 0) ? NUDGE_STRENGTH : -NUDGE_STRENGTH;
                newAngle += nudge * Time.deltaTime;
            }
            floor.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }

    void ShrinkFloor()
    {
        if (floor == null) // Safety check
            return;

        gameTime += Time.deltaTime;
        float newWidth = Mathf.Max(minFloorWidth, initialFloorScale.x - (gameTime * shrinkRate));
        currentFloorScale = new Vector2(newWidth, initialFloorScale.y);
        floor.localScale = new Vector3(currentFloorScale.x, currentFloorScale.y, 1);
    }

    void OnDestroy()
    {
        timerStyle = null;
    }

    void CheckCirclePosition()
    {
        if (circle != null && circle.position.y < deathY)
        {
            SaveHighScore(gameTime);
            ResetGame();
        }
    }

    void ResetGame()
    {
        gameTime = 0;
        currentFloorScale = initialFloorScale;
        floor.localScale = new Vector3(currentFloorScale.x, currentFloorScale.y, 1);

        if (circle != null)
        {
            circle.position = circleStartPos;
            var rb = circle.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
            }
        }
    }

    public void ResetGameTime()
    {
        gameTime = 0f;
        currentFloorScale = initialFloorScale;
        if (floor != null)
        {
            floor.localScale = new Vector3(currentFloorScale.x, currentFloorScale.y, 1);
            floor.rotation = Quaternion.identity;
        }
        if (circle != null)
        {
            circle.position = circleStartPos;
            var rb = circle.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    void OnGUI()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        GUI.color = Color.white;
        GUI.Label(new Rect(20, 15, 200, 40), $"Time: {gameTime:F2}", timerStyle);
        GUI.Label(new Rect(20, 60, 200, 30), "High Scores:", highScoreStyle);
        for (int i = 0; i < 3; i++)
        {
            if (highScores[i] > 0)
            {
                GUI.Label(new Rect(20, 85 + (i * 20), 200, 30), $"{i + 1}. {highScores[i]:F2}", highScoreStyle);
            }
        }
    }
}
