using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Damage Tool", menuName = "Scriptable Objects/Tools/Damage Tool")]
public class DamageToolSO : ToolBaseSO
{
    [SerializeField] private bool m_DealLethalDamage = false;
    [SerializeField] private float m_Damage = 100f;

    public override void UseTool(TileData targetTile)
    {
        if (targetTile == null) return;
        DamageEntities(targetTile);

        int radius = PlayerToolboxManager.Instance.BrushSize / 2;
        if (radius > 0)
        {
            var tiles = MapTileManager.Instance.GetTilesInRadius(targetTile, radius);
            foreach (var tile in tiles)
            {
                DamageEntities(tile);
            }
        }
    }

    private void DamageEntities(TileData targetTile)
    {
        List<EntityBase> entitiesToDamage = new List<EntityBase>(targetTile.Entities);
        foreach (var entity in entitiesToDamage)
        {
            float damage = m_DealLethalDamage ? entity.MaxEnergy : m_Damage;
            entity.TakeDamage(damage);
        }
    }
}
