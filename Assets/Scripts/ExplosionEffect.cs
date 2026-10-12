using System;
using System.Collections;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private Animator animator;

    [SerializeField] private float lifeTime = 0.6f;

    private Action<ExplosionEffect> returnToPoolAction;

    private Coroutine returnCoroutine;

    public void SetReturnToPoolAction(
        Action<ExplosionEffect> returnAction)
    {
        returnToPoolAction = returnAction;
    }

    public void Play(Vector3 position)
    {
        transform.position = position;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        // Luôn restart animation từ đầu.
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        returnCoroutine =
            StartCoroutine(ReturnAfterDelay());
    }

    private IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(lifeTime);

        returnToPoolAction?.Invoke(this);
    }

    public void ResetForPool()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }
}