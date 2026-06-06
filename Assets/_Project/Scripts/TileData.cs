using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public Vector2Int TileIndex { get; private set; }
    public Vector3 WorldPosition { get; private set; }

    public float Humidity { get; private set; } = 0f; // 0 to 100
    public float Nutrients { get; private set; } = 0f; // 0 to 100

    private HashSet<EntityBase> m_EntitiesOnTile = new();

    public TileData(Vector2Int tileIndex, Vector3 worldPosition)
    {
        TileIndex = tileIndex;
        WorldPosition = worldPosition;
    }

    #region Public Methods
    public void AddHumidity(float amount)
    {
        Humidity = Mathf.Clamp(Humidity + amount, 0f, 100f);
    }

    public void AddNutrients(float amount)
    {
        Nutrients = Mathf.Clamp(Nutrients + amount, 0f, 100f);
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
    #endregion
}