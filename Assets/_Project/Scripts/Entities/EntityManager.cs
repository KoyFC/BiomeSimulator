using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntitySpawnData
{
    [field: SerializeField] public EntityDataSO EntityData { get; private set; }
    [field: SerializeField] public int Amount { get; private set; }
    [field: SerializeField] public bool SpawnCluster { get; private set; } // 3x3
}

public class EntityManager : Singleton<EntityManager>
{
    [Header("Spawn Settings")]
    [SerializeField] private bool m_SpawnOnStart;
    [SerializeField] private EntitySpawnData[] m_EntitiesToSpawn;
    private List<EntityBase> m_SpawnedEntities = new();
    private float m_CurrentEntityAlpha = 1f;

    private Dictionary<Type, int> m_EntityCounts = new();

    public static event Action<string> OnGameOver;

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

    public void SetAllEntitiesAlpha(float alpha)
    {
        m_CurrentEntityAlpha = alpha;
        foreach (EntityBase entity in m_SpawnedEntities)
        {
            entity.SetAlpha(alpha);
        }
    }

    public void AddEntity(EntityBase entity)
    {
        m_SpawnedEntities.Add(entity);
        entity.SetAlpha(m_CurrentEntityAlpha);

        Type entityType = entity.GetType();
        {
            if (!m_EntityCounts.ContainsKey(entityType))
            {
                m_EntityCounts[entityType] = 0;
            }
            m_EntityCounts[entityType]++;
        }
    }

    public void RemoveEntity(EntityBase entity)
    {
        m_SpawnedEntities.Remove(entity);

        Type entityType = entity.GetType();
        if (m_EntityCounts.ContainsKey(entityType))
        {
            m_EntityCounts[entityType]--;
            if (m_EntityCounts[entityType] <= 0)
            {
                m_EntityCounts.Remove(entityType);
            }

            CheckGameOver();
        }
    }

    private void CheckGameOver()
    {
        if (!m_EntityCounts.ContainsKey(typeof(PlantBase)))
        {
            OnGameOver?.Invoke("All plants died.");
        }
        else if (!m_EntityCounts.ContainsKey(typeof(Herbivore)))
        {
            OnGameOver?.Invoke("All herbivores died.");
        }
    }
}
