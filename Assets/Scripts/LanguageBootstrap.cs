using UnityEngine;

public class LanguageBootstrap : MonoBehaviour
{
    void Awake()
    {
        Localization.LoadLanguage();
    }
}