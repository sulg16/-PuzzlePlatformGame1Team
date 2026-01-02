using System;
using UnityEngine;
public enum DayPhase
{
    Evening,
    Night,
    Morning
}
public class WorldTimeManager : MonoBehaviour
{
    public static WorldTimeManager Instance { get; private set; }

    public bool IsRunning => _running;

    public event Action<DayPhase> OnPhaseChanged;

    [Header("Duration (seconds)")]
    [SerializeField] private float totalDuration = 600f;   // 전체: 10분
    [SerializeField] private float eveningRatio = 0.33f;   // 저녁 비율
    [SerializeField] private float nightRatio = 0.34f;     // 새벽 비율

    public event Action OnTimeExpired;

    public float Elapsed { get; private set; }
    public float Remaining => Mathf.Max(0f, totalDuration - Elapsed);
    public DayPhase CurrentPhase { get; private set; } = DayPhase.Evening;

    private bool _running;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartIfNotRunning()
    {
        if (_running) return;
        StartTime();
    }

    private void Update()
    {
        if (!_running) return;

        Elapsed += Time.deltaTime;

        var phase = EvaluatePhase(Elapsed / totalDuration);
        if (phase != CurrentPhase)
        {
            CurrentPhase = phase;
            OnPhaseChanged?.Invoke(CurrentPhase);
        }

        if (Elapsed >= totalDuration)
        {
            Elapsed = totalDuration;
            _running = false;
            OnTimeExpired?.Invoke();
        }



        if (_running && Mathf.FloorToInt(Elapsed) % 10 == 0)
            Debug.Log($"[WorldTime] Elapsed={Elapsed:F1}, Phase={CurrentPhase}");
    }

    public void StartTime()
    {
        Elapsed = 0f;
        CurrentPhase = DayPhase.Evening;
        _running = true;
        OnPhaseChanged?.Invoke(CurrentPhase); // 시작 시 한번 발행
    }

    public void StopTime() => _running = false;

    private DayPhase EvaluatePhase(float normalized01)
    {
        normalized01 = Mathf.Clamp01(normalized01);

        float eveningEnd = eveningRatio;
        float nightEnd = eveningRatio + nightRatio;

        if (normalized01 < eveningEnd) return DayPhase.Evening;
        if (normalized01 < nightEnd) return DayPhase.Night;
        return DayPhase.Morning;
    }
}
