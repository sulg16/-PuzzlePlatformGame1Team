using UnityEngine;
using UnityEngine.UI;

public class LanguageSettingUI : MonoBehaviour
{

    public Button koreanButton;
    public Button englishButton;

    public Color selectedColor = Color.white;
    public Color normalColor = Color.gray;

    private void OnEnable()
    {
        Localization.OnLanguageChanged += HandleLanguageChanged;
        UpdateButtonVisual();
    }

    private void OnDisable()
    {
        Localization.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged()
    {
        UpdateButtonVisual();

    }

    public void OnClickKorean()
    {
        Localization.SetLanguage(Language.Korean);
    }

    public void OnClickEnglish()
    {
        Localization.SetLanguage(Language.English);
    }

    private void UpdateButtonVisual()
    {
        bool isKorean = Localization.CurrentLanguage == Language.Korean;

        SetButtonState(koreanButton, isKorean);
        SetButtonState(englishButton, !isKorean);
    }

    private void SetButtonState(Button button, bool isSelected)
    {
        var colors = button.colors;
        colors.normalColor = isSelected ? selectedColor : normalColor;
        colors.highlightedColor = isSelected ? selectedColor : normalColor;
        button.colors = colors;

        button.interactable = !isSelected;
    }
}
