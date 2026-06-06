using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public Vector2Int TileIndex { get; private set; }
    public Vector3 WorldPosition { get; private set; }

    public float Humidity { get; private set; } = 0f;
    public const float MAX_HUMIDITY = 100f;
    public float Nutrients { get; private set; } = 0f;
    public const float MAX_NUTRIENTS = 100f;

    private HashSet<EntityBase> m_EntitiesOnTile = new();
    public bool IsOccupied => m_EntitiesOnTile.Count > 0;

    public TileData(Vector2Int tileIndex, Vector3 worldPosition)
    {
        TileIndex = tileIndex;
        WorldPosition = worldPosition;
    }

    #region Public Methods
    public void AddHumidity(float amount)
    {
        Humidity = Mathf.Clamp(Humidity + amount, 0f, MAX_HUMIDITY);
    }

    public void AddNutrients(float amount)
    {
        Nutrients = Mathf.Clamp(Nutrients + amount, 0f, MAX_NUTRIENTS);
    }

    public void AddEntity(EntityBase entity)
    {
        if (entity == null) return;
        m_EntitiesOnTile.Add(entity);
    }

    public void RemoveEntity(EntityBase entity)
    {
        if (entity == null) return;
        m_EntitiesOnTile.Remove(entity);
    }

    public IConsumable GetConsumable(ConsumableType type)
    {
        foreach (EntityBase entity in m_EntitiesOnTile)
        {
            if (entity is IConsumable consumable && consumable.ConsumableType == type)
            {
                return consumable;
            }
        }
        return null;
    }
    #endregion
}