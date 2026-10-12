using UnityEngine;
using UnityEngine.Pool;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Prefabs")]
    [SerializeField]
    private AsteroidController smallAsteroidPrefab;

    [SerializeField]
    private AsteroidController mediumAsteroidPrefab;

    [SerializeField]
    private AsteroidController largeAsteroidPrefab;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform asteroidContainer;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 1.2f;
    [SerializeField] private float spawnOutsideOffset = 1.2f;
    [SerializeField] private float verticalSpawnPadding = 0.4f;

    [Header("Target Randomness")]
    [SerializeField] private float targetRandomX = 1.5f;
    [SerializeField] private float targetRandomY = 2.0f;

    [Header("Movement")]
    [SerializeField] private float minSpeed = 3f;
    [SerializeField] private float maxSpeed = 5f;

    [SerializeField]
    private float minRotationSpeed = 40f;

    [SerializeField]
    private float maxRotationSpeed = 140f;

    [Header("Spawn Weights")]
    [SerializeField] private float smallWeight = 0.45f;
    [SerializeField] private float mediumWeight = 0.35f;
    [SerializeField] private float largeWeight = 0.20f;

    [Header("Pool")]
    [SerializeField] private int defaultPoolCapacity = 6;
    [SerializeField] private int maxPoolSize = 20;

    [Header("Game Systems")]
    [SerializeField]
    private ExplosionPool explosionPool;

    [SerializeField]
    private ScoreManager scoreManager;

    private Camera mainCamera;

    private ObjectPool<AsteroidController> smallPool;
    private ObjectPool<AsteroidController> mediumPool;
    private ObjectPool<AsteroidController> largePool;

    private float spawnTimer;

    private void Awake()
    {
        mainCamera = Camera.main;

        smallPool =
            CreatePool(smallAsteroidPrefab);

        mediumPool =
            CreatePool(mediumAsteroidPrefab);

        largePool =
            CreatePool(largeAsteroidPrefab);
    }

    private void Start()
    {
        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnAsteroid();

            spawnTimer = spawnInterval;
        }
    }

    private ObjectPool<AsteroidController> CreatePool(
        AsteroidController prefab)
    {
        ObjectPool<AsteroidController> pool = null;

        pool = new ObjectPool<AsteroidController>(
            createFunc: () =>
            {
                AsteroidController asteroid =
                    Instantiate(
                        prefab,
                        asteroidContainer
                    );

                asteroid.SetReturnToPoolAction(
                    item => pool.Release(item)
                );


                asteroid.SetDestroyedAction(
                    (position, score) =>
                    {
                        if (explosionPool != null)
                        {
                            explosionPool.PlayExplosion(
                                position
                            );
                        }

                        if (scoreManager != null)
                        {
                            scoreManager.AddScore(
                                score
                            );
                        }
                    }
                );

                asteroid.gameObject.SetActive(false);

                return asteroid;
            },

            actionOnGet: asteroid =>
            {
                asteroid.gameObject.SetActive(true);
            },

            actionOnRelease: asteroid =>
            {
                asteroid.ResetForPool();
                asteroid.gameObject.SetActive(false);
            },

            actionOnDestroy: asteroid =>
            {
                Destroy(asteroid.gameObject);
            },

            collectionCheck: true,
            defaultCapacity: defaultPoolCapacity,
            maxSize: maxPoolSize
        );

        return pool;
    }

    private void SpawnAsteroid()
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

        Vector2 targetPosition =
            GetRandomTargetPosition(
                left,
                right,
                bottom,
                top
            );

        Vector2 direction =
            (targetPosition - spawnPosition)
            .normalized;

        float speed =
            Random.Range(
                minSpeed,
                maxSpeed
            );

        float rotationSpeed =
            Random.Range(
                minRotationSpeed,
                maxRotationSpeed
            );

        if (Random.value < 0.5f)
        {
            rotationSpeed *= -1f;
        }

        AsteroidController asteroid =
            GetRandomAsteroid();

        asteroid.Launch(
            spawnPosition,
            direction,
            speed,
            rotationSpeed
        );
    }

    private Vector2 GetRandomTargetPosition(
        float left,
        float right,
        float bottom,
        float top)
    {
        float targetX =
            player.position.x
            + Random.Range(
                -targetRandomX,
                targetRandomX
            );

        float targetY =
            player.position.y
            + Random.Range(
                -targetRandomY,
                targetRandomY
            );

        targetX = Mathf.Clamp(
            targetX,
            left + 0.5f,
            right - 0.5f
        );

        targetY = Mathf.Clamp(
            targetY,
            bottom + 0.5f,
            top - 0.5f
        );

        return new Vector2(
            targetX,
            targetY
        );
    }

    private AsteroidController GetRandomAsteroid()
    {
        float totalWeight =
            smallWeight
            + mediumWeight
            + largeWeight;

        float randomValue =
            Random.Range(
                0f,
                totalWeight
            );

        if (randomValue < smallWeight)
        {
            return smallPool.Get();
        }

        randomValue -= smallWeight;

        if (randomValue < mediumWeight)
        {
            return mediumPool.Get();
        }

        return largePool.Get();
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