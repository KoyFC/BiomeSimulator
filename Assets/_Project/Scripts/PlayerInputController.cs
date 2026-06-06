using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : Singleton<PlayerInputController>
{
    private static readonly Key[] s_SlotKeys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9, Key.Digit0 };

    [SerializeField] private InputActionReference m_MoveAction;

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
    #endregion

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

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Mouse.current.position.value;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }

    #region Slot Selection
    public static int GetSlotKeyPressed(int maxSlots)
    {
        int count = Mathf.Min(maxSlots, s_SlotKeys.Length);
        for (int i = 0; i < count; i++)
        {
            if (Keyboard.current[s_SlotKeys[i]].wasPressedThisFrame)
                return i;
        }
        return -1;
    }

    public static int GetMouseScroll()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) return -1;
        if (scroll < 0f) return 1;
        return 0;
    }
    #endregion
}