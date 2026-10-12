using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private float speed = 12f;

    [SerializeField]
    private float lifetime = 3f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.linearVelocity =
            transform.right * speed; 

        Destroy(
            gameObject,
            lifetime
        );
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        AsteroidController asteroid =
            other.GetComponentInParent<
                AsteroidController>();

        if (asteroid == null)
        {
            return;
        }

        asteroid.HitByLaser();

        Destroy(gameObject);
    }
}