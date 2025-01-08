using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    public GameObject spikePrefab;

    [Header("Spawn Timing")]
    public float initialSpawnInterval = 0.7f;
    public float spawnIntervalMin = 0.3f;
    public float spawnIntervalDecreaseRate = 0.015f;
    public float multiSpikeTimeThreshold = 60f;

    [Header("Spike Movement")]
    public float initialSpikeSpeed = 7f;
    public float spikeSpeedIncreaseRate = 0.2f;

    private float spawnInterval;
    private float spikeSpeed;
    private float timer = 0f;

    void Start()
    {
        spawnInterval = initialSpawnInterval;
        spikeSpeed = initialSpikeSpeed;
    }

    void Update()
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnSpikes();
            timer = 0f;
            spawnInterval = Mathf.Max(spawnIntervalMin, spawnInterval - spawnIntervalDecreaseRate);
            spikeSpeed += spikeSpeedIncreaseRate;
        }
    }

    void SpawnSpikes()
    {
        if (spikePrefab != null)
        {
            float groundY = -3.01f;  // adjust this to your ground's y-coordinate if necessary
            float startX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 1f;

            // Determine group count based on game time
            var controller = FindObjectOfType<WristCurlsGameController>();
            float currentTime = controller != null ? controller.CurrentGameTime : 0f;
            int groupCount = 1;
            if (currentTime > multiSpikeTimeThreshold)
            {
                // After multiSpikeTimeThreshold seconds, allow groups of 1 to 3 spikes
                groupCount = Random.Range(1, 4);
            }

            float spikeWidth = spikePrefab.GetComponent<SpriteRenderer>()?.bounds.size.x ?? 1f;
            float spacing = spikeWidth + 0.5f;

            for (int i = 0; i < groupCount; i++)
            {
                Vector3 spawnPosition = new Vector3(startX + i * spacing, groundY, 0f);
                GameObject spike = Instantiate(spikePrefab, spawnPosition, Quaternion.identity);
                Rigidbody2D rb = spike.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.gravityScale = 0;
                    rb.linearVelocity = new Vector2(-spikeSpeed, 0);
                }
                Destroy(spike, 10f);
            }
        }
    }
}
