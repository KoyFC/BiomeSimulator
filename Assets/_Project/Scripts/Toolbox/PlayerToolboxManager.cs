using System;
using UnityEngine;

public class PlayerToolboxManager : Singleton<PlayerToolboxManager>
{
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
        if (tile == null) return;

        // TODO: Do something
    }
}
