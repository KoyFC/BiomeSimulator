using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fertilize Event", menuName = "Scriptable Objects/Climate Events/Fertilize Event")]
public class FertilizeEventSO : ClimateEventBaseSO
{
    [field: SerializeField] public override string EventName { get; protected set; } = "Fertilize";
    [field: SerializeField] public override int DurationTicks { get; protected set; } = 10;

    [SerializeField] private float m_NutrientsPerTick = 5f;

    private IReadOnlyList<TileData> m_Tiles;

    public override void StartEvent()
    {
        base.StartEvent();
        m_Tiles = MapTileManager.Instance.Tiles;
    }

    protected override void ExecuteTickEffect()
    {
        foreach (TileData tile in m_Tiles)
        {
            if (tile != null)
            {
                tile.AddNutrients(m_NutrientsPerTick);
            }
        }
    }
}
