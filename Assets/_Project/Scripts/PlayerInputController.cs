using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private InputActionReference m_MoveAction;

    public static event Action<Vector3> OnPlayerClicked;
    public static event Action<Vector2> OnPlayerMoveInput;

    #region Unity Methods
    private void OnEnable()
    {
        m_MoveAction.action.Enable();
        m_MoveAction.action.performed += OnMovePerformed;
        m_MoveAction.action.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        m_MoveAction.action.Disable();
        m_MoveAction.action.performed -= OnMovePerformed;
        m_MoveAction.action.canceled -= OnMoveCanceled;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick();
        }
    }
    #endregion

    private void HandleLeftClick()
    {
        Vector3 mousePosition = Mouse.current.position.value;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 worldPosition = hitInfo.point;
            OnPlayerClicked?.Invoke(worldPosition);
        }
    }

    #region Callbacks
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        OnPlayerMoveInput?.Invoke(input);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        OnPlayerMoveInput?.Invoke(Vector2.zero);
    }
    #endregion
}