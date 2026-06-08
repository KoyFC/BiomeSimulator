using UnityEngine;

// Basically the same as herbivores but looks for meat instead of plants
public class Carnivore : AnimalBase
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

        TileData[] surroundingTiles = MapTileManager.Instance.GetSurroundingTiles(m_CurrentTile);
        TileData bestTile = null;
        float highestEnergy = 0f;
        foreach (TileData tile in surroundingTiles)
        {
            if (tile == null || !tile.IsWalkable) continue;

            IConsumable tileConsumable = tile.GetConsumable(ConsumableType.MEAT);
            if (tileConsumable != null && tileConsumable.AvailableEnergy > highestEnergy)
            {
                highestEnergy = tileConsumable.AvailableEnergy;
                bestTile = tile;
            }
        }

        if (bestTile != null)
        {
            MoveToTile(bestTile);
        }
        else
        {
            // Next tick, the carnivore will move to a random tile and enter this state again
            m_CurrentState = AnimalState.WANDER;
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
}