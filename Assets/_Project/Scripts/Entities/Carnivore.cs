using UnityEngine;

// Almost the same as herbivores but looks for meat instead of plants
public class Carnivore : AnimalBase, IThreat
{
    protected override void HandleSearchFoodState()
    {
        if (m_CurrentTile == null) return;

        IConsumable consumable = m_CurrentTile.GetConsumable(ConsumableType.MEAT);
        if (consumable != null)
        {
            m_CurrentState = AnimalState.EAT;
            return;
        }

        TileData[] tilesInRadius = MapTileManager.Instance.GetTilesInRadius(m_CurrentTile, 2);
        TileData bestTile = null;
        float highestEnergy = 0f;
        foreach (TileData tile in tilesInRadius)
        {
            if (tile == null || !tile.IsWalkable) continue;

            IConsumable tileConsumable = tile.GetConsumable(ConsumableType.MEAT);
            if (tileConsumable != null && tileConsumable.AvailableEnergy > highestEnergy)
            {
                highestEnergy = tileConsumable.AvailableEnergy;
                bestTile = tile;
            }
        }

        if (bestTile == null)
        {
            m_CurrentState = AnimalState.WANDER; // Next tick, the herbivore will move to a random tile and enter this state again
            return;
        }

        if ((m_CurrentTile.TileIndex - bestTile.TileIndex).sqrMagnitude <= 2)
        {
            MoveToTile(bestTile);
        }
        else
        {
            // Find the tile that gets closer to the best tile
            TileData[] surroundingTiles = MapTileManager.Instance.GetSurroundingTiles(m_CurrentTile);
            TileData nextStepTile = null;
            float minDistance = float.MaxValue;
            foreach (TileData adjacentTile in surroundingTiles)
            {
                if (adjacentTile != null && adjacentTile.IsWalkable)
                {
                    float dist = (adjacentTile.TileIndex - bestTile.TileIndex).sqrMagnitude;
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nextStepTile = adjacentTile;
                    }
                }
            }

            if (nextStepTile != null) MoveToTile(nextStepTile);
        }
    }

    protected override void HandleEatState()
    {
        if (m_CurrentTile == null) return;

        float energySpace = m_MaxEnergy - m_Energy;
        if (energySpace <= 0f)
        {
            m_CurrentState = AnimalState.WANDER;
            return;
        }

        IConsumable consumable = m_CurrentTile.GetConsumable(ConsumableType.MEAT);
        if (consumable == null)
        {
            m_CurrentState = AnimalState.SEARCH_FOOD;
            return;
        }

        float energyToEat = Mathf.Min(m_EnergyBiteSize, Mathf.Min(consumable.AvailableEnergy, energySpace));
        float energyConsumed = consumable.Consume(energyToEat);
        AddEnergy(energyConsumed * m_DigestionEfficiency);

        // The prey may have died during Consume()
        bool preyDied = consumable is EntityBase entity && entity.IsDead;

        bool isSatisfied = m_Energy >= m_MaxEnergy * m_StopEatingThreshold;
        bool isAboveCriticalHunger = m_Energy >= m_MaxEnergy * m_HungerThreshold;
        bool hasFoodLeft = !preyDied && consumable.AvailableEnergy > 0f;

        if (isSatisfied)
        {
            m_CurrentState = AnimalState.WANDER;
        }
        else if (isAboveCriticalHunger && !hasFoodLeft)
        {
            m_CurrentState = AnimalState.WANDER;
        }
        else if (!hasFoodLeft)
        {
            m_CurrentState = AnimalState.SEARCH_FOOD;
        }
    }

    #region IThreat
    public ThreatType ThreatType => ThreatType.PREDATOR;

    protected override bool Fears(ThreatType type)
    {
        return type == ThreatType.ENVIRONMENT;
    }
    #endregion
}