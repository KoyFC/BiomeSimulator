using System;
using UnityEngine;

public class TickManager : LazySingleton<TickManager>
{
    public static event Action OnTick;
    public static float TickTime => Instance.m_TickTime;

    [SerializeField, Min(0.01f)] private float m_TickTime = 0.5f;
    [SerializeField, Min(0f)] private float m_TimeScale = 1f;
    private float m_Timer = 0f;
    public float TimeScale => m_TimeScale;

    private bool m_GameOver = false;

    #region Unity Methods
    private void Start()
    {
        m_Timer = m_TickTime;

        EntityManager.OnGameOver += OnGameOver;
    }

    protected override void OnDestroy()
    {
        EntityManager.OnGameOver -= OnGameOver;
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

    private void OnGameOver(string message)
    {
        SetTimeScale(0f);
        m_GameOver = true;
    }

    public void SetTimeScale(float newTimeScale)
    {
        if (m_GameOver) return;
        m_TimeScale = Mathf.Max(0f, newTimeScale);
    }
}
