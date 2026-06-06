using UnityEngine;

[CreateAssetMenu(fileName = "Water Tool", menuName = "Scriptable Objects/Tools/Water Tool")]
public class WaterToolSO : ToolBaseSO
{
    [SerializeField, Min(0f)] private float m_WaterAmount = 10f;

    public override void UseTool(TileData targetTile)
    {
        if (targetTile == null) return;
        targetTile.AddHumidity(m_WaterAmount);
    }
}
