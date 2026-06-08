using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Lightning Strike Event", menuName = "Scriptable Objects/Climate Events/Lightning Strike")]
public class LightningStrikeEventSO : ClimateEventBaseSO
{
    [Header("Lightning Settings")]
    [SerializeField] private EntityDataSO m_FireEntityData;

    public override void StartEvent()
    {
        base.StartEvent();

        if (m_FireEntityData == null)
        {
            Debug.LogWarning("[Lightning] Fire Entity Data is missing in LightningStrikeEventSO");
            return;
        }

        var tiles = MapTileManager.Instance.Tiles;
        var plantTiles = tiles.Where(t => t != null && t.HasEntityOfType<PlantBase>()).ToList();

        if (plantTiles.Count > 0)
        {
            TileData startTile = plantTiles[Random.Range(0, plantTiles.Count)];

            Pool<EntityBase> pool = EntityManager.Instance.GetPoolForEntity(m_FireEntityData);
            EntityBase fire = pool.Get();

            fire.transform.SetPositionAndRotation(startTile.WorldPosition, Quaternion.identity);
            fire.Initialize(startTile, m_FireEntityData);
        }
    }

    protected override void ExecuteTickEffect() { }
}
