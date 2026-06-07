using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum VisualizationMode { NORMAL, NUTRIENTS, HUMIDITY }

public class TileOverlayManager : Singleton<TileOverlayManager>
{
    [SerializeField] private Material m_OverlayMaterial;
    [SerializeField, Range(0f, 1f)] private float m_EntityAlphaInOverlay = 0.5f;

    private Texture2D m_OverlayTexture = null;
    private bool m_IsDirty = false;

    private VisualizationMode m_CurrentMode = VisualizationMode.NORMAL;
    private const string OVERLAY_TEXTURE_NAME = "_OverlayTexture";
    private const string OVERLAY_VISUALIZATION_NORMAL = "_OVERLAYMODE_NORMAL";
    private const string OVERLAY_VISUALIZATION_NUTRIENTS = "_OVERLAYMODE_NUTRIENTS";
    private const string OVERLAY_VISUALIZATION_HUMIDITY = "_OVERLAYMODE_HUMIDITY";

    public static event Action<VisualizationMode> OnVisualizationModeChanged;

    private void Start()
    {
        Vector2Int tileQuantity = MapTileManager.Instance.TileQuantity;
        m_OverlayTexture = new Texture2D(tileQuantity.x, tileQuantity.y);
        m_OverlayTexture.filterMode = FilterMode.Point;

        var tiles = MapTileManager.Instance.Tiles;
        foreach (TileData tile in tiles)
        {
            UpdateOverlayForTile(tile);
        }
        m_OverlayTexture.Apply();

        SetVisualizationMode(m_CurrentMode);

        Shader.SetGlobalTexture(OVERLAY_TEXTURE_NAME, m_OverlayTexture);

        TileData.OnTileDataChanged += UpdateOverlayForTile;
    }

    protected override void OnDestroy()
    {
        TileData.OnTileDataChanged -= UpdateOverlayForTile;

        SetVisualizationMode(VisualizationMode.NORMAL);
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            VisualizationMode nextMode = (VisualizationMode)(((int)m_CurrentMode + 1) % Enum.GetValues(typeof(VisualizationMode)).Length);
            SetVisualizationMode(nextMode);
            m_CurrentMode = nextMode;
        }
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

        Color overlayColor = new Color(0f, nutrientsNormalized, humidityNormalized, 1f);
        Vector2Int tileQuantity = MapTileManager.Instance.TileQuantity;
        int flippedX = (tileQuantity.x - 1) - tileData.TileIndex.x;
        int flippedY = (tileQuantity.y - 1) - tileData.TileIndex.y;
        m_OverlayTexture.SetPixel(flippedX, flippedY, overlayColor);
        m_IsDirty = true;
    }

    public void SetVisualizationMode(VisualizationMode mode)
    {
        m_OverlayMaterial.DisableKeyword(OVERLAY_VISUALIZATION_NORMAL);
        m_OverlayMaterial.DisableKeyword(OVERLAY_VISUALIZATION_NUTRIENTS);
        m_OverlayMaterial.DisableKeyword(OVERLAY_VISUALIZATION_HUMIDITY);

        switch (mode)
        {
            case VisualizationMode.NORMAL:
                m_OverlayMaterial.EnableKeyword(OVERLAY_VISUALIZATION_NORMAL);
                EntityManager.Instance.SetAllEntitiesAlpha(1f);
                break;
            case VisualizationMode.NUTRIENTS:
                m_OverlayMaterial.EnableKeyword(OVERLAY_VISUALIZATION_NUTRIENTS);
                EntityManager.Instance.SetAllEntitiesAlpha(m_EntityAlphaInOverlay);
                break;
            case VisualizationMode.HUMIDITY:
                m_OverlayMaterial.EnableKeyword(OVERLAY_VISUALIZATION_HUMIDITY);
                EntityManager.Instance.SetAllEntitiesAlpha(m_EntityAlphaInOverlay);
                break;
        }

        OnVisualizationModeChanged?.Invoke(mode);
    }
}
