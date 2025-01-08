
using UnityEngine;

public class SceneInitManager : MonoBehaviour
{
    private void Start()
    {
        var rotationController = FindObjectOfType<WristRotationGameController>();
        if (rotationController != null)
        {
            rotationController.ResetGameTime();
            rotationController.enabled = true;
        }

        var curlsController = FindObjectOfType<WristCurlsGameController>();
        if (curlsController != null)
        {
            curlsController.ResetGame();
            curlsController.enabled = true;
        }
    }
}