using UnityEngine;

public class TileOverlayManager : Singleton<TileOverlayManager>
{
    private Texture2D m_OverlayTexture = null;
    private bool m_IsDirty = false;

    private void Start()
    {
        Vector2Int tileQuantity = MapTileManager.Instance.TileQuantity;
        m_OverlayTexture = new Texture2D(tileQuantity.x, tileQuantity.y);
        m_OverlayTexture.filterMode = FilterMode.Point;

        TileData.OnTileDataChanged += HandleTileDataChanged;
    }

    protected override void OnDestroy()
    {
        TileData.OnTileDataChanged -= HandleTileDataChanged;
    }

    private void HandleTileDataChanged(TileData tileData)
    {
        UpdateOverlayForTile(tileData);
    }

    private void LateUpdate()
    {
        if (m_IsDirty)
        {
            m_OverlayTexture.Apply();
            m_IsDirty = false;
        }
    }

    private void UpdateOverlayForTile(TileData tileData)
    {
        float humidityNormalized = tileData.Humidity / TileData.MAX_HUMIDITY;
        float nutrientsNormalized = tileData.Nutrients / TileData.MAX_NUTRIENTS;

        Color overlayColor = new Color(0f, nutrientsNormalized, humidityNormalized, 0.5f);
        m_OverlayTexture.SetPixel(tileData.TileIndex.x, tileData.TileIndex.y, overlayColor);
        m_IsDirty = true;
    }
}
