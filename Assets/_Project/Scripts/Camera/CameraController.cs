using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera m_CinemachineCamera = null;
    [SerializeField] private float m_MoveSpeed = 5f;

    private Vector2 m_MoveInput = Vector2.zero;

    #region Unity Methods
    private void Awake()
    {
        if (m_CinemachineCamera == null) m_CinemachineCamera = GetComponent<CinemachineCamera>();
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
        HandleMovement();
    }
    #endregion

    #region Helpers
    private void HandleMovement()
    {
        if (m_MoveInput == Vector2.zero) return;

        Vector3 moveDirection = new Vector3(m_MoveInput.x, 0, m_MoveInput.y);
        m_CinemachineCamera.transform.position += moveDirection * m_MoveSpeed * Time.deltaTime;
    }
    #endregion

    #region Callbacks
    private void OnPlayerMoveInput(Vector2 input)
    {
        m_MoveInput = input;
    }
    #endregion
}
