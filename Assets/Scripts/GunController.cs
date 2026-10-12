using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Rotation")]
    [SerializeField] private float rotationSpeed = 100f;

    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 45f;

    private float currentAngle;

    private void Start()
    {
        currentAngle = 0f;
    }

    private void Update()
    {
        RotateGun();
    }

    private void RotateGun()
    {
        float rotateInput = 0f;

        // W: quay súng lên
        if (Input.GetKey(KeyCode.W))
        {
            rotateInput = 1f;
        }

        // S: quay súng xuống
        else if (Input.GetKey(KeyCode.S))
        {
            rotateInput = -1f;
        }

        currentAngle += rotateInput * rotationSpeed * Time.deltaTime;

        currentAngle = Mathf.Clamp(
            currentAngle,
            minAngle,
            maxAngle
        );

        transform.localRotation =
            Quaternion.Euler(0f, 0f, currentAngle);
    }
}