using System;
using UnityEngine;

public class TickManager : LazySingleton<TickManager>
{
    public static event Action OnTick;
    public static float TickTime => Instance.m_TickTime;

    [SerializeField, Min(0.01f)] private float m_TickTime = 0.5f;
    [SerializeField, Min(0f)] private float m_TimeScale = 1f;
    private float m_Timer = 0f;

    #region Unity Methods
    private void Start()
    {
        m_Timer = m_TickTime;
    }

    private void Update()
    {
        m_Timer -= Time.deltaTime * m_TimeScale;
        if (m_Timer <= 0f)
        {
            m_Timer += m_TickTime;
            OnTick?.Invoke();
        }
    }
    #endregion

    public void SetTimeScale(float newTimeScale)
    {
        m_TimeScale = Mathf.Max(0f, newTimeScale);
    }
}
