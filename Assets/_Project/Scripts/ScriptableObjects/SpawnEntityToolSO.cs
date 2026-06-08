using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Spawn Entity Tool", menuName = "Scriptable Objects/Tools/Spawn Entity Tool")]
public class SpawnEntityTool : ToolBaseSO
{
    [SerializeField] private EntityDataSO m_EntityData;

    public override void UseTool(TileData targetTile)
    {
        if (targetTile == null || m_EntityData == null) return;
        TryPlaceRock(targetTile);

        int radius = PlayerToolboxManager.Instance.BrushSize / 2;
        if (radius > 0)
        {
            var tiles = MapTileManager.Instance.GetTilesInRadius(targetTile, radius);
            foreach (var tile in tiles)
            {
                TryPlaceRock(tile);
            }
        }

    }

    private void TryPlaceRock(TileData targetTile)
    {
        if (targetTile.HasEntityOfType<AnimalBase>()) return;
        if (targetTile.HasEntityOfType<RockEntity>()) return;

        List<EntityBase> entitiesToKill = new List<EntityBase>(targetTile.Entities);
        foreach (var entity in entitiesToKill)
        {
            entity.TakeDamage(entity.MaxEnergy);
        }

        Pool<EntityBase> pool = EntityManager.Instance.GetPoolForEntity(m_EntityData);
        EntityBase rock = pool.Get();

        rock.transform.SetPositionAndRotation(targetTile.WorldPosition, Quaternion.identity);
        rock.Initialize(targetTile, m_EntityData);
    }
}
