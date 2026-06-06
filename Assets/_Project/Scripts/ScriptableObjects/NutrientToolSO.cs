using UnityEngine;

[CreateAssetMenu(fileName = "Nutrient Tool", menuName = "Scriptable Objects/Tools/Nutrient Tool")]
public class NutrientToolSO : ToolBaseSO
{
    [SerializeField, Min(0f)] private float m_NutrientAmount = 10f;

    public override void UseTool(TileData targetTile)
    {
        if (targetTile == null) return;
        targetTile.AddNutrients(m_NutrientAmount);
    }
}
