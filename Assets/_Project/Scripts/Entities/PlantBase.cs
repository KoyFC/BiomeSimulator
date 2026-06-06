using UnityEngine;

public class PlantBase : EntityBase, IConsumable
{
    [Header("Plant")]
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

        if (m_Energy >= m_MaxEnergy * m_ReproductionThreshold) TryReproduce();
    }

    protected virtual void TryReproduce()
    {
        TileData emptyNeighbor = MapTileManager.Instance.GetRandomEmptyNeighbor(m_CurrentTile);
        if (emptyNeighbor == null) return;

        PlantBase child = Instantiate(gameObject, emptyNeighbor.WorldPosition, Quaternion.identity).GetComponent<PlantBase>();
        child.Initialize(emptyNeighbor);

        AddEnergy(-m_MaxEnergy * 0.5f);
    }

    #region IConsumable
    public float AvailableEnergy => m_Energy;
    public ConsumableType ConsumableType => ConsumableType.PLANT;

    public float Consume(float biteSize)
    {
        float energyToGive = Mathf.Min(biteSize, m_Energy);
        ConsumeEnergy(energyToGive);
        return energyToGive;
    }
    #endregion
}
