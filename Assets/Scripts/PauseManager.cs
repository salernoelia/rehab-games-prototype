using UnityEngine;

using UnityEngine.SceneManagement;
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    private PauseOverlay currentOverlay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Removed: DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && SceneManager.GetActiveScene().name != "MainMenu")
        {
            TogglePause();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ForceUnpause(); // Always unpause when a new scene is loaded
        if (scene.name == "MainMenu")
        {
            if (currentOverlay != null)
                Destroy(currentOverlay.gameObject);
            currentOverlay = null;
            return;
        }

        if (currentOverlay == null)
        {
            GameObject overlayObj = new GameObject("PauseOverlay");
            currentOverlay = overlayObj.AddComponent<PauseOverlay>();
            // Removed: DontDestroyOnLoad(overlayObj);
        }

        // Reset timer in potential game controllers
        FindObjectOfType<WristRotationGameController>()?.ResetGameTime();
    }

    public void TogglePause()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0 : 1;

            if (!IsPaused)
            {
                // Ensure game resumes properly
                Instance.ForceUnpause();
            }
        }
    }

    public void ForceUnpause()
    {
        IsPaused = false;
        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}