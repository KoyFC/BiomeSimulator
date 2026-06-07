using UnityEngine;

[CreateAssetMenu(fileName = "Water Tool", menuName = "Scriptable Objects/Tools/Water Tool")]
public class WaterToolSO : ToolBaseSO
{
    [Header("Water")]
    [SerializeField, Min(0f)] private float m_WaterAmount = 10f;

    public override void UseTool(TileData targetTile)
    {
        if (targetTile == null) return;
        targetTile.AddHumidity(m_WaterAmount);

        int radius = PlayerToolboxManager.Instance.BrushSize / 2;
        if (radius > 0)
        {
            var tiles = MapTileManager.Instance.GetTilesInRadius(targetTile, radius);
            foreach (var tile in tiles)
            {
                tile.AddHumidity(m_WaterAmount);
            }
        }
    }
}
