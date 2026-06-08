using UnityEngine;

public class Herbivore : AnimalBase, IConsumable
{
    protected override void HandleSearchFoodState()
    {
        if (m_CurrentTile == null) return;

        IConsumable consumable = m_CurrentTile.GetConsumable(ConsumableType.PLANT);
        if (consumable != null)
        {
            m_CurrentState = AnimalState.EAT;
            return;
        }

        // Could cause issues if multiple herbivores try to go to the same tile but it's fine for now
        TileData[] tilesInRadius = MapTileManager.Instance.GetTilesInRadius(m_CurrentTile, 2);
        TileData bestTile = null;
        float highestEnergy = 0f;
        foreach (TileData tile in tilesInRadius)
        {
            if (tile == null || !tile.IsWalkable) continue;

            IConsumable tileConsumable = tile.GetConsumable(ConsumableType.PLANT);
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

        IConsumable consumable = m_CurrentTile.GetConsumable(ConsumableType.PLANT);
        if (consumable == null)
        {
            m_CurrentState = AnimalState.SEARCH_FOOD;
            return;
        }

        float energyToEat = Mathf.Min(m_EnergyBiteSize, Mathf.Min(consumable.AvailableEnergy, energySpace));
        float energyConsumed = consumable.Consume(energyToEat);
        AddEnergy(energyConsumed * m_DigestionEfficiency);

        bool isSatisfied = m_Energy >= m_MaxEnergy * m_StopEatingThreshold;
        bool isAboveCriticalHunger = m_Energy >= m_MaxEnergy * m_HungerThreshold;
        bool hasFoodLeft = consumable.AvailableEnergy > 0f;

        if (isSatisfied)
        {
            m_CurrentState = AnimalState.WANDER;
        }
        else if (isAboveCriticalHunger && !hasFoodLeft) // ik it can be in the same if as the last one but it's more clear to read this way
        {
            m_CurrentState = AnimalState.WANDER;
        }
        else if (!hasFoodLeft)
        {
            m_CurrentState = AnimalState.SEARCH_FOOD;
        }
    }

    protected override bool Fears(ThreatType type)
    {
        return type == ThreatType.ENVIRONMENT || type == ThreatType.PREDATOR;
    }

    #region IConsumable
    public float AvailableEnergy => m_Energy;
    public ConsumableType ConsumableType => ConsumableType.MEAT;

    public float Consume(float biteSize)
    {
        float energyToGive = Mathf.Min(biteSize, m_Energy);
        ConsumeEnergy(energyToGive);
        return energyToGive;
    }
    #endregion
}