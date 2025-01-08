using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class WristCurlsPlayerController : MonoBehaviour
{
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    private float jumpCooldown = 0.2f;
    private float lastJumpTime = -10f;
    public WristCurlsGameController gameController;
    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    void Update()
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;
        if (OscReceiver.Instance == null) return;

        float gyroZ = OscReceiver.Instance.gyroZ;

        if (isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            if (Mathf.Abs(gyroZ) > 0.4f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false;
                lastJumpTime = Time.time;
                StartCoroutine(RotateSprite(-90f, 0.6f));
            }
        }
    }

    IEnumerator RotateSprite(float angle, float duration)
    {
        float startAngle = transform.eulerAngles.z;
        float endAngle = startAngle + angle;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, currentAngle);
            yield return null;
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, endAngle);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Spike"))
        {
            gameController?.ResetGame();
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = initialPosition;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        isGrounded = true;
    }
}
