using UnityEngine;

// Almost the same as herbivores but looks for meat instead of plants
public class Carnivore : AnimalBase, IThreat
{
    protected override void HandleSearchFoodState()
    {
        SearchForFood(ConsumableType.MEAT);
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