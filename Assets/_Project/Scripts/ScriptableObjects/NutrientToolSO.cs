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

        if (Radius > 0)
        {
            var tiles = MapTileManager.Instance.GetTilesInRadius(targetTile, Radius);
            foreach (var tile in tiles)
            {
                tile.AddNutrients(m_NutrientAmount);
            }
        }
    }
}
