using UnityEngine;

[CreateAssetMenu(fileName = "Nutrient Tool", menuName = "Scriptable Objects/Tools/Nutrient Tool")]
public class NutrientToolSO : ToolBaseSO
{
    [Header("Nutrients")]
    [SerializeField, Min(0f)] private float m_NutrientAmount = 10f;

    public override void UseTool(TileData targetTile)
    {
        if (targetTile == null) return;
        targetTile.AddNutrients(m_NutrientAmount);

        int radius = PlayerToolboxManager.Instance.BrushSize / 2;
        if (radius > 0)
        {
            var tiles = MapTileManager.Instance.GetTilesInRadius(targetTile, radius);
            foreach (var tile in tiles)
            {
                tile.AddNutrients(m_NutrientAmount);
            }
        }
    }
}
