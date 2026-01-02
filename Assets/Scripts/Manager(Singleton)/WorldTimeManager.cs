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
    private bool _expiredFired = false;

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

        if (!_expiredFired && Elapsed >= totalDuration)
        {
            Elapsed = totalDuration;
            _running = false;
            _expiredFired = true;

            Debug.Log("[WorldTime] Time Expired fired");
            OnTimeExpired?.Invoke();
        }


        if (_running && Mathf.FloorToInt(Elapsed) % 10 == 0)
            Debug.Log($"[WorldTime] Elapsed={Elapsed:F1}, Phase={CurrentPhase}");
    }

    public void StartTime()
    {
        Elapsed = 0f;
        _expiredFired = false;
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

    public int GetCurrentHour()
    {
        // 게임 시작 시각 (21시 = 밤 9시)
        const int startHour = 21;

        // 전체 게임 시간을 "시간 단위"로 환산
        float totalGameHours = 10f; // 9시 → 7시 = 10시간

        // Elapsed 비율 (0~1)
        float t = Mathf.Clamp01(Elapsed / totalDuration);

        // 현재 시각 계산 (21 → 31)
        float rawHour = startHour + t * totalGameHours;

        int hour = Mathf.FloorToInt(rawHour);

        // 24시 넘어가면 다시 0~23으로
        if (hour >= 24)
            hour -= 24;

        return hour;
    }
    public void GetCurrentClock(out int hour, out int minute)
    {
        const float startHour = 21f;     // 21:00 (밤 9시)
        const float totalGameHours = 10f; // 21:00 -> 07:00 = 10시간

        float t = Mathf.Clamp01(Elapsed / totalDuration);
        float raw = startHour + t * totalGameHours; // 21 -> 31(=24+7)

        int rawHourInt = Mathf.FloorToInt(raw);
        float frac = raw - rawHourInt;

        hour = rawHourInt % 24; // 24 넘어가면 0~23으로
        minute = Mathf.FloorToInt(frac * 60f);

        // 경계값 보정(이론상 60이 나올 수 있어 안전 처리)
        if (minute >= 60)
        {
            minute = 0;
            hour = (hour + 1) % 24;
        }
    }

}
