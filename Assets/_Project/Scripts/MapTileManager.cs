using UnityEngine;

public class MapTileManager : MonoBehaviour
{
    [SerializeField] private Vector2 m_MapSize = new Vector2(100, 100);
    [SerializeField] private Vector2Int m_TileQuantity = new Vector2Int(10, 10);

    private float MapWidth => m_MapSize.x;
    private float MapHeight => m_MapSize.y;
    private Vector2 TileSize => new Vector2(MapWidth / m_TileQuantity.x, MapHeight / m_TileQuantity.y);

    private void OnDrawGizmos()
    {
        Vector2 tileSize = TileSize;

        for (int x = 0; x < m_TileQuantity.x; x++)
        {
            for (int y = 0; y < m_TileQuantity.y; y++)
            {
                Vector3 tilePosition = new Vector3(x * tileSize.x, 0, y * tileSize.y) - new Vector3(MapWidth / 2, 0, MapHeight / 2);
                Gizmos.DrawWireCube(tilePosition + new Vector3(tileSize.x / 2, 0, tileSize.y / 2), new Vector3(tileSize.x, 0.1f, tileSize.y));
            }
        }
    }
}
