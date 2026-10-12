using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Screen Boundary")]
    [SerializeField] private Transform rightBoundaryPoint;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Camera mainCamera;

    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ReadMovementInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadMovementInput()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontal = 1f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            vertical = -1f;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            vertical = 1f;
        }

        moveInput = new Vector2(horizontal, vertical).normalized;
    }

    private void MovePlayer()
    {
        Vector2 nextPosition =
            rb.position +
            moveInput * moveSpeed * Time.fixedDeltaTime;

        Vector3 bottomLeft =
            mainCamera.ViewportToWorldPoint(
                new Vector3(0f, 0f, 0f)
            );

        Vector3 topRight =
            mainCamera.ViewportToWorldPoint(
                new Vector3(1f, 1f, 0f)
            );

        Bounds bounds = playerCollider.bounds;

        // Khoảng cách từ tâm Player đến mép trái Collider
        float leftOffset =
            rb.position.x - bounds.min.x;

        // Khoảng cách từ tâm Player đến mép dưới Collider
        float bottomOffset =
            rb.position.y - bounds.min.y;

        // Khoảng cách từ tâm Player đến mép trên Collider
        float topOffset =
            bounds.max.y - rb.position.y;

        // Mép phải sử dụng đầu nòng súng thay vì Collider
        float rightOffset;

        if (rightBoundaryPoint != null)
        {
            rightOffset =
                rightBoundaryPoint.position.x - rb.position.x;
        }
        else
        {
            // Fallback nếu quên gán FirePoint
            rightOffset =
                bounds.max.x - rb.position.x;
        }

        nextPosition.x = Mathf.Clamp(
            nextPosition.x,
            bottomLeft.x + leftOffset,
            topRight.x - rightOffset
        );

        nextPosition.y = Mathf.Clamp(
            nextPosition.y,
            bottomLeft.y + bottomOffset,
            topRight.y - topOffset
        );

        rb.MovePosition(nextPosition);
    }
}