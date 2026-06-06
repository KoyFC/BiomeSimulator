using UnityEngine;

public class AnimalBase : EntityBase
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float m_TimeToMoveBetweenTiles = 0.5f;
    private float m_MoveTimer = 0f;
    protected bool IsMoving => m_MoveTimer < m_TimeToMoveBetweenTiles;

    protected Vector3 m_TargetPosition = Vector3.zero;

    public override void Initialize(TileData startingTile)
    {
        base.Initialize(startingTile);
        m_MoveTimer = m_TimeToMoveBetweenTiles;
    }

    protected override void Update()
    {
        base.Update();

        if (m_CurrentTile == null) return;

        if (IsMoving)
        {
            m_MoveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(m_MoveTimer / m_TimeToMoveBetweenTiles);
            transform.position = Vector3.Lerp(transform.position, m_TargetPosition, t);
        }
    }

    protected void MoveToTile(TileData tile)
    {
        if (tile == null) return;

        SetCurrentTile(tile);

        m_TargetPosition = tile.WorldPosition;
        m_MoveTimer = 0f;
    }
}
