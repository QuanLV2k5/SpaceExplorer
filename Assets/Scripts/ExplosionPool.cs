using UnityEngine;
using UnityEngine.Pool;

public class ExplosionPool : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField]
    private ExplosionEffect explosionPrefab;

    [Header("Container")]
    [SerializeField]
    private Transform explosionContainer;

    [Header("Pool")]
    [SerializeField]
    private int defaultCapacity = 5;

    [SerializeField]
    private int maxPoolSize = 15;

    private ObjectPool<ExplosionEffect> explosionPool;

    private void Awake()
    {
        explosionPool =
            new ObjectPool<ExplosionEffect>(
                CreateExplosion,
                OnGetExplosion,
                OnReleaseExplosion,
                OnDestroyExplosion,
                true,
                defaultCapacity,
                maxPoolSize
            );
    }

    private ExplosionEffect CreateExplosion()
    {
        ExplosionEffect explosion =
            Instantiate(
                explosionPrefab,
                explosionContainer
            );

        explosion.SetReturnToPoolAction(
            item => explosionPool.Release(item)
        );

        explosion.gameObject.SetActive(false);

        return explosion;
    }

    private void OnGetExplosion(
        ExplosionEffect explosion)
    {
        explosion.gameObject.SetActive(true);
    }

    private void OnReleaseExplosion(
        ExplosionEffect explosion)
    {
        explosion.ResetForPool();

        explosion.gameObject.SetActive(false);
    }

    private void OnDestroyExplosion(
        ExplosionEffect explosion)
    {
        Destroy(explosion.gameObject);
    }

    public void PlayExplosion(Vector3 position)
    {
        ExplosionEffect explosion =
            explosionPool.Get();

        explosion.Play(position);
    }
}