using UnityEngine;

public class PlantBase : EntityBase, IConsumable
{
    [Header("Plant")]
    [SerializeField, Min(0f)] private float m_NutrientAbsorptionRate = 5f; // Per tick
    [SerializeField, Min(0f)] private float m_WaterAbsorptionRate = 3f;

    protected override void OnTick()
    {
        base.OnTick();
        if (IsDead) return;
        if (m_CurrentTile == null) return;

        float energySpace = m_MaxEnergy - m_Energy;
        if (energySpace <= 0f) return;

        // The plant tries to absorb water to determine how much it actually will grow
        float waterToAbsorb = Mathf.Min(m_WaterAbsorptionRate, m_CurrentTile.Humidity);
        m_CurrentTile.AddHumidity(-waterToAbsorb);
        float growthMultiplier = (m_WaterAbsorptionRate > 0f) ? Mathf.Clamp01(waterToAbsorb / m_WaterAbsorptionRate) : 1f;
        growthMultiplier = growthMultiplier.Remap(0f, 1f, 0.5f, 1.5f); // Grow better with more water, but still grow a bit without it

        // Try to absorb as many nutrients as possible without wasting the tile's nutrients
        float nutrientsToAbsorb = Mathf.Min(m_NutrientAbsorptionRate, Mathf.Min(m_CurrentTile.Nutrients, energySpace));
        m_CurrentTile.AddNutrients(-nutrientsToAbsorb);
        AddEnergy(nutrientsToAbsorb * m_DigestionEfficiency * growthMultiplier);

        if (m_Energy >= m_MaxEnergy * m_ReproductionThreshold) TryReproduce();
    }

    protected virtual void TryReproduce()
    {
        TileData emptyNeighbor = MapTileManager.Instance.GetRandomNeighborWithout<PlantBase>(m_CurrentTile);
        if (emptyNeighbor == null) return;

        // PlantBase child = Instantiate(gameObject, emptyNeighbor.WorldPosition, Quaternion.identity).GetComponent<PlantBase>();
        Pool<EntityBase> pool = EntityManager.Instance.GetPoolForEntity(EntityData);
        if (pool == null) return;
        PlantBase child = pool.Get() as PlantBase;
        child.Initialize(emptyNeighbor, EntityData);

        AddEnergy(-m_MaxEnergy * m_ReproductionEnergyCost);
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
