using System.Collections.Generic;
using UnityEngine;

public class FireEntity : EntityBase, IWaterReactor, IThreat
{
    [Header("Fire")]
    [SerializeField] private float m_DamagePerTick = 10f;
    [SerializeField] private int m_ExpandEveryTicks = 2;
    private int m_ExpandTickCounter = 0;

    public ThreatType ThreatType => ThreatType.ENVIRONMENT;

    private bool m_HasFuel = false;

    protected override void OnTick()
    {
        base.OnTick();
        if (IsDead || m_CurrentTile == null) return;

        m_HasFuel = false;

        DamageEntities();

        // If there's no fuel we want the fire to die quickly
        if (!m_HasFuel)
        {
            ConsumeEnergy(m_MaxEnergy * 0.5f);
        }

        m_ExpandTickCounter++;
        if (m_ExpandTickCounter >= m_ExpandEveryTicks)
        {
            m_ExpandTickCounter = 0;
            Expand();
        }
    }

    private void DamageEntities()
    {
        List<EntityBase> entitiesToDamage = new List<EntityBase>(m_CurrentTile.Entities);
        foreach (EntityBase entity in entitiesToDamage)
        {
            if (entity == this || entity.IsDead) continue;

            // If there is a plant the fire can consume it for energy
            if (entity is PlantBase)
            {
                m_HasFuel = true;
                AddEnergy(m_DamagePerTick * m_DigestionEfficiency);
            }

            entity.TakeDamage(m_DamagePerTick);
        }
    }

    private void Expand()
    {
        TileData emptyNeighbor = MapTileManager.Instance.GetRandomNeighborWithout<FireEntity>(m_CurrentTile);
        if (emptyNeighbor != null && emptyNeighbor.HasEntityOfType<PlantBase>())
        {
            Pool<EntityBase> pool = EntityManager.Instance.GetPoolForEntity(EntityData);
            FireEntity newFire = pool.Get() as FireEntity;

            if (newFire != null)
            {
                newFire.transform.SetPositionAndRotation(emptyNeighbor.WorldPosition, Quaternion.identity);
                newFire.Initialize(emptyNeighbor, EntityData);

                AddEnergy(-m_MaxEnergy * m_ReproductionEnergyCost);
            }
        }
    }

    public void OnWaterAdded(float amount)
    {
        TakeDamage(amount * 3f);
    }
}
