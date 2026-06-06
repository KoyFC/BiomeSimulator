using UnityEngine;

public class TileData
{
    public Vector2Int TileIndex { get; private set; }
    public Vector3 WorldPosition { get; private set; }

    public float Humidity { get; private set; } = 0f; // 0 to 100
    public float Nutrients { get; private set; } = 0f; // 0 to 100

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
    #endregion
}