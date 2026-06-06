using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerToolboxManager : Singleton<PlayerToolboxManager>
{
    [SerializeField] private ToolBaseSO[] m_Tools;
    private int m_CurrentToolIndex = 0;
    private TileData m_SelectedTile = null;

    private void Start()
    {
        if (m_Tools.Length > 0) m_CurrentToolIndex = 0;
    }

    private void Update()
    {
        HandleToolSwitchInput();

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mouseWorldPosition = PlayerInputController.Instance.GetMouseWorldPosition();
            TileData tile = MapTileManager.Instance.GetTileForWorldPosition(mouseWorldPosition);

            if (tile != null && tile != m_SelectedTile)
            {
                m_SelectedTile = tile;
                OnPlayerClicked(mouseWorldPosition);
            }
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            m_SelectedTile = null;
        }
    }

    private void HandleToolSwitchInput()
    {
        int slotIndex = PlayerInputController.GetSlotKeyPressed(m_Tools.Length);
        if (slotIndex >= 0)
        {
            m_CurrentToolIndex = slotIndex;
        }

        int scroll = PlayerInputController.GetMouseScroll();
        if (scroll != 0)
        {
            int toolCount = m_Tools.Length;
            m_CurrentToolIndex = (m_CurrentToolIndex + scroll + toolCount) % toolCount;
        }
    }

    private void OnPlayerClicked(Vector3 worldPosition)
    {
        TileData tile = MapTileManager.Instance.GetTileForWorldPosition(worldPosition);
        if (tile == null || m_Tools[m_CurrentToolIndex] == null) return;

        m_Tools[m_CurrentToolIndex].UseTool(tile);
    }
}
