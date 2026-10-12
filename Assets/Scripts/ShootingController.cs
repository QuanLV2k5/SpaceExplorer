using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float fireCooldown = 0.25f;

    private float nextFireTime;

    private void Update()
    {
        HandleShooting();
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space) &&
            Time.time >= nextFireTime)
        {
            Shoot();

            nextFireTime = Time.time + fireCooldown;
        }
    }

    private void Shoot()
    {
        Instantiate(
            laserPrefab,
            firePoint.position,
            firePoint.rotation
        );
    }
}