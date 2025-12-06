using UnityEngine;

public enum Language
{
    Korean = 0,
    English = 1
}

public static class Localization
{
    public static Language CurrentLanguage { get; private set; } = Language.Korean;

    public static event System.Action OnLanguageChanged;

    public static void LoadLanguage()
    {
        int saved = PlayerPrefs.GetInt("Language", 0);
        CurrentLanguage = (Language)saved;
    }

    public static void SetLanguage(Language lang)
    {
        if (CurrentLanguage == lang) return;

        CurrentLanguage = lang;
        PlayerPrefs.SetInt("Language", (int)lang);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke();
    }
}