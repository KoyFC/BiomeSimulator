using UnityEngine;

public class Herbivore : AnimalBase, IConsumable
{
    protected override void HandleSearchFoodState()
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleEatState()
    {
        if (m_CurrentTile == null) return;

        float energySpace = m_MaxEnergy - m_Energy;
        if (energySpace <= 0f) return;

        IConsumable consumable = m_CurrentTile.GetConsumable(ConsumableType.PLANT);
        if (consumable == null)
        {
            m_CurrentState = AnimalState.SEARCH_FOOD;
            return;
        }

        float energyToEat = Mathf.Min(m_EnergyBiteSize, Mathf.Min(consumable.AvailableEnergy, energySpace));
        float energyConsumed = consumable.Consume(energyToEat);
        AddEnergy(energyConsumed * m_DigestionEfficiency);

        if (m_Energy >= m_HungerThreshold)
        {
            m_CurrentState = AnimalState.WANDER;
        }
        else if (consumable.AvailableEnergy <= 0f)
        {
            m_CurrentState = AnimalState.SEARCH_FOOD;
        }
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