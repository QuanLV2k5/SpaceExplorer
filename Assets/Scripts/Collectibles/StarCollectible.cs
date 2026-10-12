using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class StarCollectible : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private int scoreValue = 50;

    [Header("Despawn")]
    [SerializeField] private float viewportDespawnMargin = 0.15f;

    private Rigidbody2D rb;
    private Camera mainCamera;

    private Action<StarCollectible> returnToPoolAction;
    private Action<int> collectedAction;

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
        Action<StarCollectible> action)
    {
        returnToPoolAction = action;
    }

    public void SetCollectedAction(
        Action<int> action)
    {
        collectedAction = action;
    }

    public void Launch(
        Vector2 spawnPosition,
        float speed)
    {
        transform.position = spawnPosition;
        transform.rotation = Quaternion.identity;

        // Chỉ bay ngang từ phải sang trái.
        rb.linearVelocity =
            Vector2.left * speed;

        hasEnteredScreen = false;
        isActiveFromPool = true;
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (!isActiveFromPool)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        collectedAction?.Invoke(scoreValue);

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

        // Star chủ yếu đi sang trái,
        // nhưng kiểm tra toàn bộ bounds để an toàn.
        bool outsideScreen =
            viewportPosition.x <
                -viewportDespawnMargin ||

            viewportPosition.x >
                1f + viewportDespawnMargin ||

            viewportPosition.y <
                -viewportDespawnMargin ||

            viewportPosition.y >
                1f + viewportDespawnMargin;

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
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}