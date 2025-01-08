using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseOverlay : MonoBehaviour
{
    private GUIStyle pauseStyle;
    private string gyroDataDisplay = "";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Destroy(gameObject);
        }
        pauseStyle = new GUIStyle();
        pauseStyle.fontSize = 40;
        pauseStyle.normal.textColor = Color.white;
        pauseStyle.alignment = TextAnchor.MiddleCenter;

        if (OscReceiver.Instance != null)
        {
            OscReceiver.Instance.OnGyroDataReceived += UpdateGyroDisplay;
        }
    }

    private void UpdateGyroDisplay(float x, float y, float z)
    {
        gyroDataDisplay = $"X: {x:F2}\nY: {y:F2}\nZ: {z:F2}";
    }

    private void OnGUI()
    {
        if (PauseManager.Instance == null)
            return;

        if (PauseManager.Instance.IsPaused)
        {
            // Draw semi-transparent background
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            float centerY = Screen.height / 2;
            GUI.Label(new Rect(0, centerY - 100, Screen.width, 50), "PAUSED", pauseStyle);
            GUI.Label(new Rect(0, centerY, Screen.width, 50), $"Gyro Data:\n{gyroDataDisplay}", pauseStyle);
            GUI.Label(new Rect(0, centerY + 100, Screen.width, 50), "Press M for Main Menu", pauseStyle);
        }
    }

    private void Update()
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused && Input.GetKeyDown(KeyCode.M))
        {
            PauseManager.Instance.ForceUnpause(); // Ensure unpaused before scene change
            SceneManager.LoadScene("MainMenu");
        }
    }
}
