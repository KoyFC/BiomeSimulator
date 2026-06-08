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
    public ToolBaseSO CurrentTool => m_CurrentToolSlot.Tools.Length > 0 ? m_CurrentToolSlot.Tools[m_CurrentToolIndex] : null;
    private ToolSlot m_CurrentToolSlot;
    private TileData m_SelectedTile = null;

    [field: SerializeField, Min(1)] public int MaxBrushSize { get; private set; } = 11;
    public int BrushSize { get; set; } = 1;

    public static event System.Action<ToolBaseSO[]> OnToolsChanged;

    protected override void Awake()
    {
        base.Awake();
        if (m_ToolSlots.Length > 0) m_CurrentToolSlot = m_ToolSlots[0];

        TileOverlayManager.OnVisualizationModeChanged += OnVisualizationModeChanged;
        EntityManager.OnGameOver += message => enabled = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TileOverlayManager.OnVisualizationModeChanged -= OnVisualizationModeChanged;
        EntityManager.OnGameOver -= message => enabled = false;
    }

    private void Update()
    {
        HandleToolSwitchInput();
        HandleMouseScrollInput();

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
    }

    private void HandleMouseScrollInput()
    {
        if (!Keyboard.current.leftShiftKey.isPressed) return;

        int scroll = -PlayerInputController.GetMouseScroll();
        if (scroll == 0) return;

        BrushSize += scroll;
        if (BrushSize % 2 == 0) BrushSize += (scroll > 0) ? 1 : -1;
        BrushSize = Mathf.Clamp(BrushSize, 1, MaxBrushSize);
    }

    private void OnPlayerClicked(Vector3 worldPosition)
    {
        TileData tile = MapTileManager.Instance.GetTileForWorldPosition(worldPosition);
        if (tile == null || m_CurrentToolSlot.Tools.Length == 0 || m_CurrentToolSlot.Tools[m_CurrentToolIndex] == null) return;

        m_CurrentToolSlot.Tools[m_CurrentToolIndex].UseTool(tile);
    }

    public void TrySelectTool(ToolBaseSO tool)
    {
        for (int i = 0; i < m_CurrentToolSlot.Tools.Length; i++)
        {
            if (m_CurrentToolSlot.Tools[i] == tool)
            {
                m_CurrentToolIndex = i;
                return;
            }
        }
    }

    private void OnVisualizationModeChanged(VisualizationMode mode)
    {
        foreach (ToolSlot slot in m_ToolSlots)
        {
            if (slot.VisualizationMode == mode)
            {
                m_CurrentToolSlot = slot;
                m_CurrentToolIndex = 0;
                OnToolsChanged?.Invoke(m_CurrentToolSlot.Tools);
                return;
            }
        }
    }
}
