using UnityEngine;

public class ClimateManager : Singleton<ClimateManager>
{
    [SerializeField] private int m_TicksBetweenEvents = 30;
    private int m_TickCounter = 0;

    [SerializeField] private ClimateEventBaseSO[] m_ClimateEvents;
    private ClimateEventBaseSO m_CurrentEvent = null;

    public static event System.Action<ClimateEventBaseSO> OnClimateEventStarted;
    public static event System.Action<ClimateEventBaseSO> OnClimateEventEnded;

    protected override void Awake()
    {
        base.Awake();
        TickManager.OnTick += OnTick;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TickManager.OnTick -= OnTick;
    }

    private void OnTick()
    {
        if (m_CurrentEvent != null)
        {
            m_CurrentEvent.OnTick();

            if (m_CurrentEvent.IsFinished)
            {
                m_CurrentEvent.EndEvent();
                m_CurrentEvent = null;
                m_TickCounter = 0;
                OnClimateEventEnded?.Invoke(m_CurrentEvent);
            }
        }
        else
        {
            m_TickCounter++;
            if (m_TickCounter >= m_TicksBetweenEvents) StartRandomEvent();
        }
    }

    private void StartRandomEvent()
    {
        int random = Random.Range(0, m_ClimateEvents.Length);
        m_CurrentEvent = m_ClimateEvents[random];
        m_CurrentEvent.StartEvent();
        OnClimateEventStarted?.Invoke(m_CurrentEvent);
    }
}
