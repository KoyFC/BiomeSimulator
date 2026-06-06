using UnityEngine;

public abstract class PlantBase : EntityBase
{
    [SerializeField, Min(0f)] private float m_NutrientAbsorptionRate = 5f; // Per tick

    protected override void OnTick()
    {
        base.OnTick();
        if (m_CurrentTile == null) return;

        float energySpace = m_MaxEnergy - m_Energy;
        if (energySpace <= 0f) return;

        // Try to absorb as many nutrients as possible without wasting the tile's nutrients
        float nutrientsToAbsorb = Mathf.Min(m_NutrientAbsorptionRate, Mathf.Min(m_CurrentTile.Nutrients, energySpace));
        m_CurrentTile.AddNutrients(-nutrientsToAbsorb);
        AddEnergy(nutrientsToAbsorb * m_DigestionEfficiency);
    }
}
