using UnityEngine;

public abstract class AnimalBase : EntityBase
{
    protected enum AnimalState
    {
        WANDER,
        SEARCH_FOOD,
        EAT
    }

    [Header("Movement")]
    [SerializeField, Min(0f)] private float m_TimeToMoveBetweenTiles = 0.5f;
    private float m_MoveTimer = 0f;
    protected Vector3 m_TargetPosition = Vector3.zero;
    protected bool IsMoving => m_MoveTimer < m_TimeToMoveBetweenTiles;

    private AnimalState m_CurrentState = AnimalState.WANDER;

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

    protected override void OnTick()
    {
        base.OnTick();
        switch (m_CurrentState)
        {
            case AnimalState.WANDER:
                HandleWanderState();
                break;
            case AnimalState.SEARCH_FOOD:
                HandleSearchFoodState();
                break;
            case AnimalState.EAT:
                HandleEatState();
                break;
        }
    }

    #region State Handlers
    protected virtual void HandleWanderState()
    {
        TileData[] surroundingTiles = MapTileManager.Instance.GetSurroundingTiles(m_CurrentTile);

        TileData randomTile = surroundingTiles[Random.Range(0, surroundingTiles.Length)];
        if (randomTile != null) MoveToTile(randomTile);
    }

    protected virtual void HandleSearchFoodState() { }
    protected virtual void HandleEatState() { }
    #endregion
}
