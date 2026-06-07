using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera m_CinemachineCamera = null;
    [SerializeField] private float m_MoveSpeed = 5f;
    [SerializeField] private Collider m_MovementBounds = null;
    private Camera m_MainCamera = null;

    private Vector2 m_MoveInput = Vector2.zero;

    #region Unity Methods
    private void Awake()
    {
        m_MainCamera = Camera.main;
        if (m_CinemachineCamera == null) m_CinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
    }
    private void OnEnable()
    {
        PlayerInputController.OnPlayerMoveInput += OnPlayerMoveInput;
    }

    private void OnDisable()
    {
        PlayerInputController.OnPlayerMoveInput -= OnPlayerMoveInput;
    }

    private void Update()
    {
        HandleZoom();
    }

    private void LateUpdate()
    {
        HandleMovement();
    }
    #endregion

    #region Helpers
    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(m_MoveInput.x, 0, m_MoveInput.y);
        Vector3 newPosition = transform.position + moveDirection * m_MoveSpeed * Time.deltaTime;

        if (m_MovementBounds != null && m_MainCamera != null)
        {
            float size = m_CinemachineCamera.Lens.OrthographicSize;
            float aspect = m_MainCamera.aspect;
            float cameraHalfHeight = size;
            float cameraHalfWidth = size * aspect;

            float xAngle = m_MainCamera.transform.eulerAngles.x;
            if (xAngle > 5f && xAngle < 85f) // We don't want to adjust if the camera is practically flat or perpendicular
            {
                cameraHalfHeight = size / Mathf.Sin(xAngle * Mathf.Deg2Rad);
            }

            Bounds bounds = m_MovementBounds.bounds;

            float minX = bounds.min.x + cameraHalfWidth;
            float maxX = bounds.max.x - cameraHalfWidth;

            float minZ = bounds.min.z + cameraHalfHeight;
            float maxZ = bounds.max.z - cameraHalfHeight;

            // If the camera size is bigger than the bounds we just keep it centered
            if (minX > maxX)
            {
                float midX = (bounds.min.x + bounds.max.x) / 2f;
                minX = midX;
                maxX = midX;
            }
            if (minZ > maxZ)
            {
                float midZ = (bounds.min.z + bounds.max.z) / 2f;
                minZ = midZ;
                maxZ = midZ;
            }

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        }

        transform.position = newPosition;
    }

    private void HandleZoom()
    {
        if (Keyboard.current.leftShiftKey.isPressed) return;
        float scrollInput = PlayerInputController.GetMouseScroll();
        if (Mathf.Approximately(scrollInput, 0f)) return;

        m_CinemachineCamera.Lens.OrthographicSize = Mathf.Clamp(m_CinemachineCamera.Lens.OrthographicSize + scrollInput, 7f, 29f);
    }
    #endregion

    #region Callbacks
    private void OnPlayerMoveInput(Vector2 input)
    {
        m_MoveInput = input;
    }
    #endregion
}
