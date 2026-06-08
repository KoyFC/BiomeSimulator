using UnityEngine;

public class Herbivore : AnimalBase, IConsumable
{
    protected override void HandleSearchFoodState()
    {
        SearchForFood(ConsumableType.PLANT);
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