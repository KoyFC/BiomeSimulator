using UnityEngine;

public class MapTileManager : MonoBehaviour
{
    [SerializeField] private Vector2 m_MapSize = new Vector2(100, 100);
    [SerializeField] private Vector2Int m_TileQuantity = new Vector2Int(10, 10);
    private TileData[,] m_Tiles = null;

    private float MapWidth => m_MapSize.x;
    private float MapHeight => m_MapSize.y;
    private Vector2 TileSize => new Vector2(MapWidth / m_TileQuantity.x, MapHeight / m_TileQuantity.y);

    #region Unity Methods
    private void Start()
    {
        m_Tiles = new TileData[m_TileQuantity.x, m_TileQuantity.y];

        for (int x = 0; x < m_TileQuantity.x; x++)
        {
            for (int y = 0; y < m_TileQuantity.y; y++)
            {
                Vector3 worldPosition = CalculateWorldPositionForTile(x, y);
                m_Tiles[x, y] = new TileData(new Vector2Int(x, y), worldPosition);
            }
        }
    }

    private void OnDestroy()
    {
        m_Tiles = null;
    }
    #endregion

    #region Helpers
    private Vector3 CalculateWorldPositionForTile(float x, float y)
    {
        Vector2 tileSize = TileSize;
        return new Vector3(x * tileSize.x, 0, y * tileSize.y) - new Vector3(MapWidth / 2, 0, MapHeight / 2);
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        Vector2 tileSize = TileSize;

        for (int x = 0; x < m_TileQuantity.x; x++)
        {
            for (int y = 0; y < m_TileQuantity.y; y++)
            {
                Vector3 worldPosition;
                if (m_Tiles != null)
                {
                    worldPosition = m_Tiles[x, y].WorldPosition;
                }
                else
                {
                    worldPosition = CalculateWorldPositionForTile(x, y);
                }

                Gizmos.DrawWireCube(worldPosition + new Vector3(tileSize.x / 2, 0, tileSize.y / 2), new Vector3(tileSize.x, 0.1f, tileSize.y));
            }
        }
    }
    #endregion
}
