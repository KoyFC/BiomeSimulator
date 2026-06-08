using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rain Event", menuName = "Scriptable Objects/Climate Events/Rain Event")]
public class RainEventSO : ClimateEventBaseSO
{
    [field: SerializeField] public override string EventName { get; protected set; } = "Rain";
    [field: SerializeField] public override int DurationTicks { get; protected set; } = 10;

    [SerializeField] private float m_HumidityPerTick = 5f;

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
                tile.AddHumidity(m_HumidityPerTick);
            }
        }
    }
}
