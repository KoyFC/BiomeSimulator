using UnityEngine;

public class TileData
{
    public Vector2Int TileIndex { get; private set; }
    public Vector3 WorldPosition { get; private set; }

    public TileData(Vector2Int tileIndex, Vector3 worldPosition)
    {
        TileIndex = tileIndex;
        WorldPosition = worldPosition;
    }
}