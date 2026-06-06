using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public static event Action<Vector3> OnPlayerClicked;

    #region Unity Methods
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick();
        }
    }

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
    #endregion
}