using UnityEngine;
using System.Net.Sockets;
using System.Linq;
using System;
using CoreOSC.IO;

public class OscServer : MonoBehaviour
{
    private UdpClient udpClient;
    private float gyroZ;
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
    public float respawnHeight = 15f;
    public float deathY = -10f;

    private Vector2 currentFloorScale;
    private float gameTime;
    private Vector3 circleStartPos;

    void Start()
    {
        udpClient = new UdpClient(57100);
        StartCoroutine(ReceiveOscMessages());

        currentFloorScale = initialFloorScale;
        floor.localScale = new Vector3(currentFloorScale.x, currentFloorScale.y, 1);

        if (circle != null)
        {
            circleStartPos = circle.position;
        }

        LoadHighScores();

        // Initialize styles if not set
        {
            timerStyle = new GUIStyle();
            timerStyle.fontSize = 36;
            timerStyle.fontStyle = FontStyle.Bold;
            timerStyle.normal.textColor = new Color(1f, 1f, 1f, 1f); // Pure white
        }

        if (highScoreStyle == null)
        {
            highScoreStyle = new GUIStyle();
            highScoreStyle.fontSize = 24;
            highScoreStyle.fontStyle = FontStyle.Bold;
            highScoreStyle.normal.textColor = new Color(1f, 1f, 1f, 1f); // Pure white
        }
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
        // Add new score and sort
        float[] newScores = new float[4];
        Array.Copy(highScores, newScores, 3);
        newScores[3] = newScore;
        Array.Sort(newScores);
        Array.Reverse(newScores);

        // Keep top 3
        Array.Copy(newScores, highScores, 3);

        // Save to PlayerPrefs
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetFloat($"HighScore{i}", highScores[i]);
        }
        PlayerPrefs.Save();
    }

    private System.Collections.IEnumerator ReceiveOscMessages()
    {
        while (true)
        {
            var receiveTask = udpClient.ReceiveMessageAsync();
            while (!receiveTask.IsCompleted) yield return null;
            var response = receiveTask.Result;

            if (response.Address.Value == "/zigsim/gyro")
            {
                var args = response.Arguments.ToArray();
                gyroZ = Convert.ToSingle(args[2]);
            }
        }
    }

    void Update()
    {
        HandleFloorRotation();
        ShrinkFloor();
        CheckCirclePosition();
    }

    void HandleFloorRotation()
    {
        if (floor != null)
        {
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
        gameTime += Time.deltaTime;
        float newWidth = Mathf.Max(minFloorWidth, initialFloorScale.x - (gameTime * shrinkRate));
        currentFloorScale = new Vector2(newWidth, initialFloorScale.y);
        floor.localScale = new Vector3(currentFloorScale.x, currentFloorScale.y, 1);
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
    void OnGUI()
    {
        // Add semi-transparent background
        GUI.color = new Color(0, 0, 0, 0.5f);
        GUI.color = Color.white;

        // Display current time
        GUI.Label(new Rect(20, 15, 200, 40), $"Time: {gameTime:F2}", timerStyle);

        // Display high scores with tighter spacing
        GUI.Label(new Rect(20, 60, 200, 30), "High Scores:", highScoreStyle);
        for (int i = 0; i < 3; i++)
        {
            if (highScores[i] > 0)
            {
                GUI.Label(new Rect(20, 85 + (i * 20), 200, 30),
                    $"{i + 1}. {highScores[i]:F2}", highScoreStyle);
            }
        }
    }
}