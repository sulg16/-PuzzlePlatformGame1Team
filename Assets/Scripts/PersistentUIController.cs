using TMPro;
using UnityEngine;

public class PersistentUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject phoneMenuRoot;

    [Header("TMP")]
    [SerializeField] private TMP_Text timeText;

    public bool IsPhoneMenuOpen => phoneMenuRoot != null && phoneMenuRoot.activeSelf;

    private void Awake()
    {
        if (failPanel != null) failPanel.SetActive(false);
        if (phoneMenuRoot != null) phoneMenuRoot.SetActive(false);
    }

    public void ShowFail()
    {
        if (failPanel != null) failPanel.SetActive(true);
    }

    public void HideFail()
    {
        if (failPanel != null) failPanel.SetActive(false);
    }

    public void ShowPhoneMenu()
    {
        if (phoneMenuRoot != null) phoneMenuRoot.SetActive(true);
    }

    public void HidePhoneMenu()
    {
        if (phoneMenuRoot != null) phoneMenuRoot.SetActive(false);
    }

    public void SetTimeText(float remainSeconds, DayPhase phase)
    {
        if (timeText == null) return;

        string phaseKr = phase switch
        {
            DayPhase.Evening => "저녁",
            DayPhase.Night => "새벽",
            DayPhase.Morning => "아침",
            _ => "-"
        };

        int m = Mathf.FloorToInt(remainSeconds / 60f);
        int s = Mathf.FloorToInt(remainSeconds % 60f);

        timeText.text = $"{phaseKr}\n{m:00}:{s:00}";
    }
}
