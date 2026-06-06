using System;
using UnityEngine;

public class PlayerToolboxManager : Singleton<PlayerToolboxManager>
{
    [SerializeField] private ToolBaseSO[] m_Tools;
    private int m_CurrentToolIndex = 0;

    private void Start()
    {
        if (m_Tools.Length > 0) m_CurrentToolIndex = 0;
    }

    private void OnEnable()
    {
        PlayerInputController.OnPlayerClicked += OnPlayerClicked;
    }

    private void OnDisable()
    {
        PlayerInputController.OnPlayerClicked -= OnPlayerClicked;
    }

    private void Update()
    {
        HandleToolSwitchInput();
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
