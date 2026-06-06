using UnityEngine;

public class MapTileManager : Singleton<MapTileManager>
{
    [Header("Map")]
    [SerializeField] private Vector2 m_MapSize = new Vector2(100, 100);
    [SerializeField] private Vector2Int m_TileQuantity = new Vector2Int(10, 10);
    private Vector2 m_TileSize = Vector2.zero;
    private TileData[,] m_Tiles = null;

    [Header("Gizmos")]
    [SerializeField] private bool m_OnlyDrawSelected = true;
    [SerializeField] private Color m_GizmoColor = Color.red;

    private float MapWidth => m_MapSize.x;
    private float MapHeight => m_MapSize.y;
    private Vector2 TileSize => m_TileSize != Vector2.zero ? m_TileSize : CalculateTileSize();

    #region Unity Methods
    private void Start()
    {
        m_Tiles = new TileData[m_TileQuantity.x, m_TileQuantity.y];
        m_TileSize = CalculateTileSize();

        for (int x = 0; x < m_TileQuantity.x; x++)
        {
            for (int y = 0; y < m_TileQuantity.y; y++)
            {
                Vector3 worldPosition = CalculateWorldPositionForTile(x, y);
                m_Tiles[x, y] = new TileData(new Vector2Int(x, y), worldPosition);

                float randomNormalValue = (Random.value + Random.value + Random.value) / 3f; // Simulate normal distribution thanks JM
                float initialHumidity = randomNormalValue * TileData.MAX_HUMIDITY;
                m_Tiles[x, y].AddHumidity(initialHumidity);
                float initialNutrients = randomNormalValue * TileData.MAX_NUTRIENTS;
                m_Tiles[x, y].AddNutrients(initialNutrients);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        m_Tiles = null;
        m_TileSize = Vector2.zero;
    }
    #endregion

    #region Helpers
    private Vector2 CalculateTileSize()
    {
        return new Vector2(MapWidth / m_TileQuantity.x, MapHeight / m_TileQuantity.y);
    }

    private Vector3 CalculateWorldPositionForTile(float x, float y)
    {
        Vector2 tileSize = TileSize;
        return new Vector3(x * tileSize.x, 0, y * tileSize.y) - new Vector3(MapWidth / 2, 0, MapHeight / 2);
    }
    #endregion

    #region Public Methods
    public TileData GetTileForWorldPosition(Vector3 worldPosition)
    {
        return GetTileForWorldPosition(worldPosition.x, worldPosition.z);
    }

    public TileData GetTileForWorldPosition(float x, float z)
    {
        Vector2 tileSize = TileSize;
        int tileX = Mathf.FloorToInt((x + MapWidth / 2) / tileSize.x);
        int tileY = Mathf.FloorToInt((z + MapHeight / 2) / tileSize.y);

        bool isWithinX = tileX.IsWithinRange(0, m_TileQuantity.x);
        bool isWithinY = tileY.IsWithinRange(0, m_TileQuantity.y);

        if (isWithinX && isWithinY)
        {
            return m_Tiles[tileX, tileY];
        }
        else
        {
            Debug.LogWarning($"[MapTileManager] World position ({x}, {z}) is outside of the map.");
            return null;
        }
    }

    // Maybe in the future use a ref to an array
    public TileData[] GetSurroundingTiles(TileData centerTile)
    {
        if (centerTile == null) return null;

        Vector2Int centerIndex = centerTile.TileIndex;
        TileData[] surroundingTiles = new TileData[8];

        int index = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int tileX = centerIndex.x + x;
                int tileY = centerIndex.y + y;

                if (tileX.IsWithinRange(0, m_TileQuantity.x) && tileY.IsWithinRange(0, m_TileQuantity.y))
                {
                    surroundingTiles[index] = m_Tiles[tileX, tileY];
                }
                else
                {
                    surroundingTiles[index] = null;
                }
                index++;
            }
        }

        return surroundingTiles;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (m_OnlyDrawSelected) return;
        DrawGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        if (!m_OnlyDrawSelected) return;
        DrawGizmos();
    }

    private void DrawGizmos()
    {
        Vector2 tileSize = TileSize;
        Vector3 gizmoSize = new Vector3(tileSize.x, 0.1f, tileSize.y);
        Vector3 localTileCenter = new Vector3(tileSize.x / 2, 0, tileSize.y / 2);

        for (int x = 0; x < m_TileQuantity.x; x++)
        {
            for (int y = 0; y < m_TileQuantity.y; y++)
            {
                Vector3 worldPosition = (m_Tiles != null) ? m_Tiles[x, y].WorldPosition : CalculateWorldPositionForTile(x, y);
                Vector3 tileCenter = worldPosition + localTileCenter;
                Gizmos.color = m_GizmoColor;
                Gizmos.DrawWireCube(tileCenter, gizmoSize);

                if (m_Tiles != null)
                {
                    float blue = m_Tiles[x, y].Humidity / TileData.MAX_HUMIDITY;
                    float green = m_Tiles[x, y].Nutrients / TileData.MAX_NUTRIENTS;
                    Color fillColor = new Color(0, green, blue, 0.5f);
                    Gizmos.color = fillColor;
                    Gizmos.DrawCube(tileCenter, gizmoSize);
                }
            }
        }
    }
    #endregion
}
