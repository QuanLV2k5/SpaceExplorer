using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Backgrounds")]
    [SerializeField] private SpriteRenderer backgroundA;
    [SerializeField] private SpriteRenderer backgroundB;

    [Header("Scrolling")]
    [SerializeField] private float scrollSpeed = 0.8f;

    [Tooltip("Chồng nhẹ 2 background để tránh xuất hiện đường hở.")]
    [SerializeField] private float seamOverlap = 0.02f;

    [Header("Visual")]
    [SerializeField] private bool mirrorSecondBackground = true;

    private Camera mainCamera;

    private float backgroundWidth;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        SetupBackgrounds();
    }

    private void Update()
    {
        ScrollBackgrounds();
        RecycleBackgrounds();
    }

    private void SetupBackgrounds()
    {
        if (backgroundA == null || backgroundB == null)
        {
            Debug.LogError(
                "BackgroundScroller: Background A/B chưa được gán!"
            );

            enabled = false;

            return;
        }

        // Bảo đảm B có cùng scale với A.
        backgroundB.transform.localScale =
            backgroundA.transform.localScale;

        // Chiều rộng thật của background trong World Space.
        backgroundWidth =
            backgroundA.bounds.size.x;

        // Đặt B ngay bên phải A.
        backgroundB.transform.position =
            new Vector3(
                backgroundA.transform.position.x
                    + backgroundWidth
                    - seamOverlap,

                backgroundA.transform.position.y,

                backgroundA.transform.position.z
            );

        // Mirror ảnh thứ hai để giảm đường nối.
        if (mirrorSecondBackground)
        {
            backgroundB.flipX = true;
        }
    }

    private void ScrollBackgrounds()
    {
        Vector3 movement =
            Vector3.left
            * scrollSpeed
            * Time.deltaTime;

        backgroundA.transform.position += movement;
        backgroundB.transform.position += movement;
    }

    private void RecycleBackgrounds()
    {
        float cameraLeft =
            mainCamera.transform.position.x
            - mainCamera.orthographicSize
            * mainCamera.aspect;

        float halfWidth =
            backgroundWidth * 0.5f;

        // A đã ra hoàn toàn khỏi màn hình bên trái.
        if (
            backgroundA.transform.position.x
            + halfWidth
            < cameraLeft
        )
        {
            MoveToRightOf(
                backgroundA,
                backgroundB
            );
        }

        // B đã ra hoàn toàn khỏi màn hình bên trái.
        if (
            backgroundB.transform.position.x
            + halfWidth
            < cameraLeft
        )
        {
            MoveToRightOf(
                backgroundB,
                backgroundA
            );
        }
    }

    private void MoveToRightOf(
        SpriteRenderer backgroundToMove,
        SpriteRenderer otherBackground
    )
    {
        backgroundToMove.transform.position =
            new Vector3(
                otherBackground.transform.position.x
                    + backgroundWidth
                    - seamOverlap,

                backgroundToMove.transform.position.y,

                backgroundToMove.transform.position.z
            );
    }
}