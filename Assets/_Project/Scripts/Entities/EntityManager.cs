using System;
using UnityEngine;

[Serializable]
public class EntitySpawnData
{
    [field: SerializeField] public EntityDataSO EntityData { get; private set; }
    [field: SerializeField] public int Amount { get; private set; }
    [field: SerializeField] public bool SpawnCluster { get; private set; } // 3x3
}

public class EntityManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private bool m_SpawnOnStart;
    [SerializeField] private EntitySpawnData[] m_EntitiesToSpawn;

    private void Start()
    {
        if (m_SpawnOnStart)
        {
            SpawnEntities();
        }
    }

    private void SpawnEntities()
    {
        foreach (var spawnData in m_EntitiesToSpawn)
        {
            for (int i = 0; i < spawnData.Amount; i++)
            {
                TileData randomTile = MapTileManager.Instance.GetRandomEmptyTile();
                if (randomTile == null) return; // This means there are no tilesavailable

                if (spawnData.SpawnCluster)
                {
                    SpawnCluster(spawnData, randomTile);
                }
                else
                {
                    SpawnEntity(spawnData, randomTile);
                }
            }
        }
    }

    private void SpawnEntity(EntitySpawnData spawnData, TileData tile)
    {
        var entity = Instantiate(spawnData.EntityData.Prefab, tile.WorldPosition, Quaternion.identity);
        entity.Initialize(tile);
    }

    private void SpawnCluster(EntitySpawnData spawnData, TileData randomTile)
    {
        TileData[] clusterTiles = MapTileManager.Instance.GetSurroundingTiles(randomTile);
        SpawnEntity(spawnData, randomTile);
        foreach (TileData tile in clusterTiles)
        {
            if (tile != null) // When spawning clusters we don't really care if tiles are occupied. Just a design choice.
            {
                SpawnEntity(spawnData, tile);
            }
        }
    }
}
