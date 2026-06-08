using UnityEngine;
using System.Collections.Generic;

public abstract class AnimalBase : EntityBase
{
    protected enum AnimalState
    {
        WANDER,
        SEARCH_FOOD,
        EAT,
        FLEE
    }

    protected AnimalState m_CurrentState = AnimalState.WANDER;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float m_TimeToMoveBetweenTiles = 0.5f;
    private float m_MoveTimer = 0f;
    protected Vector3 m_TargetPosition = Vector3.zero;
    protected bool IsMoving => m_MoveTimer < m_TimeToMoveBetweenTiles;

    [Header("Hunger")]
    [SerializeField, Range(0f, 1f)] protected float m_HungerThreshold = 0.5f;
    [SerializeField, Range(0f, 1f)] protected float m_StopEatingThreshold = 0.9f;

    [Header("Eating")]
    [SerializeField, Min(0f)] protected float m_EnergyBiteSize = 10f; // How much energy MAX the animal can get in a tick

    [Header("Threats")]
    [SerializeField, Min(1)] protected int m_VisionRadius = 2;
    [SerializeField, Range(0f, 1f)] protected float m_PanicChance = 0.2f; // Chance to not move when fleeing
    [SerializeField, Min(1f)] protected float m_FleeEnergyMultiplier = 3f;

    protected virtual bool Fears(ThreatType type) { return false; }

    public override void Initialize(TileData startingTile, EntityDataSO entityData)
    {
        base.Initialize(startingTile, entityData);
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
        if (IsDead) return;

        EntityBase nearestThreat = CheckForNearestThreat();
        if (nearestThreat != null)
        {
            m_CurrentState = AnimalState.FLEE;
        }
        else if (m_CurrentState == AnimalState.FLEE)
        {
            m_CurrentState = AnimalState.WANDER;
        }

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
            case AnimalState.FLEE:
                HandleFleeState(nearestThreat);
                break;
        }
    }

    private EntityBase CheckForNearestThreat()
    {
        EntityBase nearestThreat = null;
        float minThreatDistance = float.MaxValue;
        TileData[] visibleTiles = MapTileManager.Instance.GetTilesInRadius(m_CurrentTile, m_VisionRadius);
        if (visibleTiles != null)
        {
            foreach (TileData tile in visibleTiles)
            {
                if (tile == null) continue;
                foreach (EntityBase entity in tile.Entities)
                {
                    if (entity is IThreat threat && Fears(threat.ThreatType))
                    {
                        float distance = (m_CurrentTile.TileIndex - tile.TileIndex).sqrMagnitude;
                        if (distance < minThreatDistance)
                        {
                            minThreatDistance = distance;
                            nearestThreat = entity;
                        }
                    }
                }
            }
        }

        return nearestThreat;
    }

    #region State Handlers
    protected virtual void HandleWanderState()
    {
        TileData[] surroundingTiles = MapTileManager.Instance.GetSurroundingTiles(m_CurrentTile);

        List<TileData> walkableTiles = new();
        foreach (TileData tile in surroundingTiles)
        {
            if (tile != null && tile.IsWalkable) walkableTiles.Add(tile);
        }

        if (walkableTiles.Count > 0)
        {
            TileData randomTile = walkableTiles[Random.Range(0, walkableTiles.Count)];
            MoveToTile(randomTile);
        }

        if (m_Energy > m_MaxEnergy * m_ReproductionThreshold)
        {
            TryReproduce();
        }

        if (m_Energy < m_MaxEnergy * m_HungerThreshold)
        {
            m_CurrentState = AnimalState.SEARCH_FOOD;
            return;
        }

    }

    protected virtual void TryReproduce()
    {
        TileData mateTile = MapTileManager.Instance.FindNeighborWithMate<AnimalBase>(m_CurrentTile, this);
        if (mateTile == null) return;

        AnimalBase mate = mateTile.FindMateOfType(this);
        if (mate == null) return;

        TileData childTile = MapTileManager.Instance.GetRandomNeighborWithout<AnimalBase>(m_CurrentTile);
        if (childTile == null) return;

        // AnimalBase child = Instantiate(gameObject, childTile.WorldPosition, Quaternion.identity).GetComponent<AnimalBase>();
        Pool<EntityBase> pool = EntityManager.Instance.GetPoolForEntity(EntityData);
        if (pool == null) return;
        AnimalBase child = pool.Get() as AnimalBase;
        child.Initialize(childTile, EntityData);

        // Both parents lose energy
        float energyCost = m_MaxEnergy * m_ReproductionEnergyCost;
        AddEnergy(-energyCost);
        mate.AddEnergy(-energyCost);
    }

    protected abstract void HandleSearchFoodState();
    protected abstract void HandleEatState();

    protected virtual void HandleFleeState(EntityBase threat)
    {
        if (threat == null || threat.CurrentTile == null) return;

        float energyCost = m_EnergyConsumptionPerSecond * TickManager.TickTime * (m_FleeEnergyMultiplier - 1f); // Subtract 1 since normal energy cost was already applied in OnTick

        if (Random.value < m_PanicChance)
        {
            ConsumeEnergy(energyCost);
            return;
        }

        // Find tile that is furthest from the threat
        TileData[] surroundingTiles = MapTileManager.Instance.GetSurroundingTiles(m_CurrentTile);
        TileData bestEscapeTile = null;
        float maxDistance = float.MinValue;

        foreach (TileData tile in surroundingTiles)
        {
            if (tile == null || !tile.IsWalkable) continue;

            float distance = (tile.TileIndex - threat.CurrentTile.TileIndex).sqrMagnitude;
            if (distance > maxDistance)
            {
                maxDistance = distance;
                bestEscapeTile = tile;
            }
        }

        if (bestEscapeTile != null) MoveToTile(bestEscapeTile);
        ConsumeEnergy(energyCost);
    }
    #endregion

}
