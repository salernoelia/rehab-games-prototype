using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GUIStyle menuStyle;
    // List available scene names. Adjust these names to match your actual scenes.
    private string[] scenes = new string[] { "WristRotation", "WristCurls" };

    private void Start()
    {
        menuStyle = new GUIStyle();
        menuStyle.fontSize = 40;
        menuStyle.normal.textColor = Color.white;
        menuStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnGUI()
    {
        int buttonWidth = 300;
        int buttonHeight = 60;
        int spacing = 20;
        float startY = Screen.height / 2 - ((buttonHeight + spacing) * scenes.Length) / 2;

        for (int i = 0; i < scenes.Length; i++)
        {
            Rect buttonRect = new Rect(
                (Screen.width - buttonWidth) / 2,
                startY + i * (buttonHeight + spacing),
                buttonWidth,
                buttonHeight
            );

            if (GUI.Button(buttonRect, scenes[i], menuStyle))
            {
                SceneManager.LoadScene(scenes[i]);
            }
        }
    }
}

