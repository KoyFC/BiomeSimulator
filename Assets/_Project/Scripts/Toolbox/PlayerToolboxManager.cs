using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerToolboxManager : Singleton<PlayerToolboxManager>
{
    [System.Serializable]
    private struct ToolSlot
    {
        [field: SerializeField] public VisualizationMode VisualizationMode { get; private set; }
        [field: SerializeField] public ToolBaseSO[] Tools { get; private set; }
    }

    [SerializeField] private ToolSlot[] m_ToolSlots;
    private int m_CurrentToolIndex = 0;
    private ToolSlot m_CurrentToolSlot;
    private TileData m_SelectedTile = null;

    protected override void Awake()
    {
        base.Awake();
        if (m_ToolSlots.Length > 0) m_CurrentToolSlot = m_ToolSlots[0];

        TileOverlayManager.OnVisualizationModeChanged += OnVisualizationModeChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TileOverlayManager.OnVisualizationModeChanged -= OnVisualizationModeChanged;
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
        int slotIndex = PlayerInputController.GetSlotKeyPressed(m_CurrentToolSlot.Tools.Length);
        if (slotIndex >= 0)
        {
            m_CurrentToolIndex = slotIndex;
        }

        int scroll = PlayerInputController.GetMouseScroll();
        if (scroll != 0)
        {
            int toolCount = m_CurrentToolSlot.Tools.Length;
            m_CurrentToolIndex = (m_CurrentToolIndex + scroll + toolCount) % toolCount;
        }
    }

    private void OnPlayerClicked(Vector3 worldPosition)
    {
        TileData tile = MapTileManager.Instance.GetTileForWorldPosition(worldPosition);
        if (tile == null || m_CurrentToolSlot.Tools[m_CurrentToolIndex] == null) return;

        m_CurrentToolSlot.Tools[m_CurrentToolIndex].UseTool(tile);
    }

    private void OnVisualizationModeChanged(VisualizationMode mode)
    {
        foreach (ToolSlot slot in m_ToolSlots)
        {
            if (slot.VisualizationMode == mode)
            {
                m_CurrentToolSlot = slot;
                m_CurrentToolIndex = 0;
                return;
            }
        }
    }
}
