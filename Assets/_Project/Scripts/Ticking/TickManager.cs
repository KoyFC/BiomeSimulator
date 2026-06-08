using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TickManager : LazySingleton<TickManager>
{
    public static event Action OnTick;
    public static float TickTime => Instance.m_TickTime;

    [SerializeField, Min(0.01f)] private float m_TickTime = 0.5f;
    [SerializeField, Min(0f)] private float m_TimeScale = 1f;
    public float TimeScale => m_TimeScale;
    private float m_Timer = 0f;
    private float m_LastTimeScale = 1f;

    private bool m_GameOver = false;
    private uint m_TickCount = 0;
    public uint TickCount => m_TickCount;

    public static event Action<float> OnTimeScaleChanged;

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
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            float newTimeScale = m_TimeScale == 0f ? m_LastTimeScale : 0f;
            SetTimeScale(newTimeScale);
        }

        m_Timer -= Time.deltaTime * m_TimeScale;
        if (m_Timer <= 0f)
        {
            m_Timer += m_TickTime;
            m_TickCount++;
            OnTick?.Invoke();
        }
    }
    #endregion

    private void OnGameOver(string message)
    {
        SetTimeScale(0f);
        m_GameOver = true;

        uint bestTickCount = (uint)PlayerPrefs.GetInt("BestTickCount", 0);
        if (m_TickCount > bestTickCount)
        {
            PlayerPrefs.SetInt("BestTickCount", (int)m_TickCount);
            PlayerPrefs.Save();
        }
    }

    public void SetTimeScale(float newTimeScale)
    {
        if (m_GameOver) return;
        m_LastTimeScale = m_TimeScale;
        m_TimeScale = Mathf.Max(0f, newTimeScale);
        OnTimeScaleChanged?.Invoke(m_TimeScale);
    }
}
