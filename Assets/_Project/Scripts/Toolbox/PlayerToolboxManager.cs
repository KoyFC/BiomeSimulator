using System;
using UnityEngine;

public class PlayerToolboxManager : Singleton<PlayerToolboxManager>
{
    [SerializeField] private ToolBaseSO[] m_Tools;
    private ToolBaseSO m_CurrentTool = null;

    private void OnEnable()
    {
        PlayerInputController.OnPlayerClicked += OnPlayerClicked;
    }

    private void OnDisable()
    {
        PlayerInputController.OnPlayerClicked -= OnPlayerClicked;
    }

    private void OnPlayerClicked(Vector3 worldPosition)
    {
        TileData tile = MapTileManager.Instance.GetTileForWorldPosition(worldPosition);
        if (tile == null || m_CurrentTool == null) return;

        m_CurrentTool.UseTool(tile);
    }
}
