using UnityEngine;
using UnityEngine.Pool;

public class StarSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField]
    private StarCollectible starPrefab;

    [Header("References")]
    [SerializeField]
    private Transform starContainer;

    [SerializeField]
    private ScoreManager scoreManager;

    [Header("Spawn Time")]
    [SerializeField]
    private float minSpawnInterval = 3f;

    [SerializeField]
    private float maxSpawnInterval = 6f;

    [Header("Spawn Position")]
    [SerializeField]
    private float spawnOutsideOffset = 1f;

    [SerializeField]
    private float verticalSpawnPadding = 0.8f;

    [Header("Movement")]
    [SerializeField]
    private float minSpeed = 2.5f;

    [SerializeField]
    private float maxSpeed = 4f;

    [Header("Pool")]
    [SerializeField]
    private int defaultPoolCapacity = 4;

    [SerializeField]
    private int maxPoolSize = 10;

    private Camera mainCamera;

    private ObjectPool<StarCollectible> starPool;

    private float spawnTimer;

    private void Awake()
    {
        mainCamera = Camera.main;

        starPool =
            new ObjectPool<StarCollectible>(
                CreateStar,
                OnGetStar,
                OnReleaseStar,
                OnDestroyStar,
                true,
                defaultPoolCapacity,
                maxPoolSize
            );
    }

    private void Start()
    {
        ResetSpawnTimer();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnStar();

            ResetSpawnTimer();
        }
    }

    private StarCollectible CreateStar()
    {
        StarCollectible star =
            Instantiate(
                starPrefab,
                starContainer
            );

        star.SetReturnToPoolAction(
            item => starPool.Release(item)
        );

        star.SetCollectedAction(
            score =>
            {
                if (scoreManager != null)
                {
                    scoreManager.AddScore(score);
                }
            }
        );

        star.gameObject.SetActive(false);

        return star;
    }

    private void OnGetStar(
        StarCollectible star)
    {
        star.gameObject.SetActive(true);
    }

    private void OnReleaseStar(
        StarCollectible star)
    {
        star.ResetForPool();

        star.gameObject.SetActive(false);
    }

    private void OnDestroyStar(
        StarCollectible star)
    {
        Destroy(star.gameObject);
    }

    private void SpawnStar()
    {
        GetCameraBounds(
            out float left,
            out float right,
            out float bottom,
            out float top
        );

        Vector2 spawnPosition =
            new Vector2(
                right + spawnOutsideOffset,

                Random.Range(
                    bottom + verticalSpawnPadding,
                    top - verticalSpawnPadding
                )
            );

        float speed =
            Random.Range(
                minSpeed,
                maxSpeed
            );

        StarCollectible star =
            starPool.Get();

        star.Launch(
            spawnPosition,
            speed
        );
    }

    private void ResetSpawnTimer()
    {
        spawnTimer =
            Random.Range(
                minSpawnInterval,
                maxSpawnInterval
            );
    }

    private void GetCameraBounds(
        out float left,
        out float right,
        out float bottom,
        out float top)
    {
        Vector3 bottomLeft =
            mainCamera.ViewportToWorldPoint(
                new Vector3(
                    0f,
                    0f,
                    0f
                )
            );

        Vector3 topRight =
            mainCamera.ViewportToWorldPoint(
                new Vector3(
                    1f,
                    1f,
                    0f
                )
            );

        left = bottomLeft.x;
        right = topRight.x;
        bottom = bottomLeft.y;
        top = topRight.y;
    }
}