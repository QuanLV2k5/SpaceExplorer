using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class AsteroidController : MonoBehaviour
{
    [Header("Asteroid Type")]
    [SerializeField] private float speedMultiplier = 1f;

    [SerializeField]
    private float rotationMultiplier = 1f;

    [SerializeField] private int scoreValue = 10;

    [Header("Despawn")]
    [SerializeField]
    private float viewportDespawnMargin = 0.15f;

    private Rigidbody2D rb;
    private Camera mainCamera;

    private Action<AsteroidController>
        returnToPoolAction;

    private Action<Vector3, int>
        destroyedAction;

    private bool isActiveFromPool;

    private bool hasEnteredScreen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckScreenBounds();
    }

    public void SetReturnToPoolAction(
        Action<AsteroidController> returnAction)
    {
        returnToPoolAction = returnAction;
    }

    public void SetDestroyedAction(
        Action<Vector3, int> action)
    {
        destroyedAction = action;
    }

    public void Launch(
        Vector2 spawnPosition,
        Vector2 direction,
        float baseSpeed,
        float baseRotationSpeed)
    {
        transform.position = spawnPosition;

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                UnityEngine.Random.Range(
                    0f,
                    360f
                )
            );

        float finalSpeed =
            baseSpeed * speedMultiplier;

        float finalRotationSpeed =
            baseRotationSpeed
            * rotationMultiplier;

        rb.linearVelocity =
            direction.normalized
            * finalSpeed;

        rb.angularVelocity =
            finalRotationSpeed;

        hasEnteredScreen = false;
        isActiveFromPool = true;
    }

    public void HitByLaser()
    {
        if (!isActiveFromPool)
        {
            return;
        }

        // Gửi vị trí explosion + điểm số.
        destroyedAction?.Invoke(
            transform.position,
            scoreValue
        );

        ReturnToPool();
    }

    private void CheckScreenBounds()
    {
        if (!isActiveFromPool ||
            mainCamera == null)
        {
            return;
        }

        Vector3 viewportPosition =
            mainCamera.WorldToViewportPoint(
                transform.position
            );

        if (!hasEnteredScreen)
        {
            bool insideScreen =
                viewportPosition.x >= 0f &&
                viewportPosition.x <= 1f &&
                viewportPosition.y >= 0f &&
                viewportPosition.y <= 1f;

            if (insideScreen)
            {
                hasEnteredScreen = true;
            }

            return;
        }

        bool outsideScreen =
            viewportPosition.x
                < -viewportDespawnMargin ||

            viewportPosition.x
                > 1f
                + viewportDespawnMargin ||

            viewportPosition.y
                < -viewportDespawnMargin ||

            viewportPosition.y
                > 1f
                + viewportDespawnMargin;

        if (outsideScreen)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        if (!isActiveFromPool)
        {
            return;
        }

        isActiveFromPool = false;

        returnToPoolAction?.Invoke(this);
    }

    public void ResetForPool()
    {
        isActiveFromPool = false;

        hasEnteredScreen = false;

        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity = 0f;
        }
    }
}