using System;
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

    public static event Action<TileData> OnTileDataChanged;

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
        OnTileDataChanged?.Invoke(this);
    }

    public void AddNutrients(float amount)
    {
        Nutrients = Mathf.Clamp(Nutrients + amount, 0f, MAX_NUTRIENTS);
        OnTileDataChanged?.Invoke(this);
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

    public bool HasEntityOfType<T>() where T : EntityBase
    {
        foreach (EntityBase entity in m_EntitiesOnTile)
        {
            if (entity is T) return true;
        }
        return false;
    }

    public T FindMateOfType<T>(T self) where T : EntityBase
    {
        foreach (EntityBase entity in m_EntitiesOnTile)
        {
            if (entity is T match && match != self && entity.GetType() == self.GetType())
            {
                return match;
            }
        }
        return null;
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