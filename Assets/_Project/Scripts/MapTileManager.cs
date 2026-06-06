using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapTileManager : Singleton<MapTileManager>
{
    [Header("Map")]
    [SerializeField] private Vector2 m_MapSize = new Vector2(100, 100);
    [SerializeField] private Vector2Int m_TileQuantity = new Vector2Int(10, 10);
    private Vector2 m_TileSize = Vector2.zero;
    private TileData[,] m_Tiles = null;
    private readonly HashSet<TileData> m_HighlightedTiles = new();

    [Header("Gizmos")]
    [SerializeField] private bool m_OnlyDrawSelected = true;
    [SerializeField] private Color m_GizmoColor = Color.red;

    private float MapWidth => m_MapSize.x;
    private float MapHeight => m_MapSize.y;
    public Vector2 TileSize => m_TileSize != Vector2.zero ? m_TileSize : CalculateTileSize();

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
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

    private void Update()
    {
        m_HighlightedTiles.Clear();

        if (m_Tiles == null) return;

        Vector3 mouseWorldPosition = PlayerInputController.Instance.GetMouseWorldPosition();
        TileData hoveredTile = GetTileForWorldPosition(mouseWorldPosition);
        if (hoveredTile == null) return;

        m_HighlightedTiles.Add(hoveredTile);

        TileData[] surroundingTiles = GetSurroundingTiles(hoveredTile);
        if (surroundingTiles != null)
        {
            for (int i = 0; i < surroundingTiles.Length; i++)
            {
                if (surroundingTiles[i] != null) m_HighlightedTiles.Add(surroundingTiles[i]);
            }
        }
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

        Vector3 tileCenter = new Vector3(tileSize.x / 2f, 0, tileSize.y / 2f);
        Vector3 worldPosition = new Vector3(x * tileSize.x, 0, y * tileSize.y) + tileCenter;
        Vector3 worldPositionCentered = worldPosition - new Vector3(MapWidth / 2, 0, MapHeight / 2);

        return worldPositionCentered;
    }
    #endregion

    #region Public Methods
    public bool IsTileHighlighted(TileData tile)
    {
        return tile != null && m_HighlightedTiles.Contains(tile);
    }

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

    public TileData GetRandomEmptyTile()
    {
        if (m_Tiles == null) return null;

        List<TileData> emptyTiles = m_Tiles.Cast<TileData>().Where(tile => tile != null && !tile.IsOccupied).ToList();
        if (emptyTiles.Count == 0) return null;
        return emptyTiles[Random.Range(0, emptyTiles.Count)];
    }

    public TileData GetRandomEmptyNeighbor(TileData centerTile)
    {
        if (centerTile == null) return null;

        TileData[] surroundingTiles = GetSurroundingTiles(centerTile);
        List<TileData> emptyNeighbors = surroundingTiles.Where(tile => tile != null && !tile.IsOccupied).ToList();

        if (emptyNeighbors.Count == 0) return null;
        return emptyNeighbors[Random.Range(0, emptyNeighbors.Count)];
    }

    public TileData GetRandomNeighborWithout<T>(TileData centerTile) where T : EntityBase
    {
        if (centerTile == null) return null;

        TileData[] surroundingTiles = GetSurroundingTiles(centerTile);
        List<TileData> validNeighbors = surroundingTiles.Where(tile => tile != null && !tile.HasEntityOfType<T>()).ToList();

        if (validNeighbors.Count == 0) return null;
        return validNeighbors[Random.Range(0, validNeighbors.Count)];
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

        for (int x = 0; x < m_TileQuantity.x; x++)
        {
            for (int y = 0; y < m_TileQuantity.y; y++)
            {
                Vector3 tileCenter = (m_Tiles != null) ? m_Tiles[x, y].WorldPosition : CalculateWorldPositionForTile(x, y);
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
