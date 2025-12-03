using UnityEngine;
using UnityEngine.UI;

public class UI_SettingPanel_Intro : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        bgmSlider.SetValueWithoutNotify(AudioManager.Instance.bgmVolume);
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.sfxVolume);

        bgmSlider.onValueChanged.AddListener(OnBgmSlider);
        sfxSlider.onValueChanged.AddListener(OnSfxSlider);
    }

    void OnDestroy()
    {
        bgmSlider.onValueChanged.RemoveListener(OnBgmSlider);
        sfxSlider.onValueChanged.RemoveListener(OnSfxSlider);
    }

    void OnBgmSlider(float value)
    {
        AudioManager.Instance.SetBgmVolume(value);
    }

    void OnSfxSlider(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);
    }
}
