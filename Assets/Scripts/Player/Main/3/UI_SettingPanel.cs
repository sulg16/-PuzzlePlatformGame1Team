using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = UnityEngine.SceneManagement.Scene;

public class UI_SettingPanel : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    GameObject start;
    GameObject save;

    PlayerController _controller;
    InventoryView _inventory;

    public void InitPanel()
    {
        start = UI_Manager.Instance._start;
        save = UI_Manager.Instance._save;
        _controller = SafeFetchHelper.GetChildOrError<PlayerController>(Player.Instance.gameObject);
        _inventory = SafeFetchHelper.GetChildOrError<InventoryView>(UI_Manager.Instance.gameObject);
    }

    public void OpenUI()
    {
        Time.timeScale = 0.1f;
        DirectionManager.Instance.LockOnCam(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void CloseUI()
    {
        Time.timeScale = 1f;
        DirectionManager.Instance.LockOnCam(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnToggleSettings()
    {
        if (!gameObject.activeSelf)
        {
            OpenUI();
            gameObject.SetActive(true);

            bgmSlider.SetValueWithoutNotify(AudioManager.Instance.bgmVolume);
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.sfxVolume);

            bgmSlider.onValueChanged.RemoveListener(OnBgmSlider);
            sfxSlider.onValueChanged.RemoveListener(OnSfxSlider);
        }

        else
        {
            CloseUI();
            gameObject.SetActive(false);

            // 리스너 등록, 실시간반영용
            bgmSlider.onValueChanged.AddListener(OnBgmSlider);
            sfxSlider.onValueChanged.AddListener(OnSfxSlider);

        }
    }
    public void OnToggleInventory()
    {
        if (!_inventory.gameObject.activeSelf)
        {
            OpenUI();
            _inventory.gameObject.SetActive(true);
        }
        else
        {
            CloseUI();
            _inventory.gameObject.SetActive(false);
        }
    }



    public void OnCloseTheScene()
    {
        SceneManager.LoadScene(0);
    }

    public void OnSave()
    {
        save.SetActive(!save.activeSelf);
    }
    public void OnLoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // 현재 씬 인덱스
        SceneManager.LoadScene(currentSceneIndex + 1); // 다음 씬으로 이동
    }
    public void OnGodMode()
    {

    }

    public void OnBgmSlider(float value)
    {
        AudioManager.Instance.SetBgmVolume(value);
    }

    public void OnSfxSlider(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);
    }

    public void OnStart()
    {
        start.SetActive(false);
        //DirectionManager.Instance.Direction_Intro();
        //AudioManager.Instance.PlayBGM("Game");

        SceneManager.LoadScene("MainScene_Floor2");
    }

    public void OnGameOver()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 플레이 모드 종료
#else
              Application.Quit(); // 빌드된 게임 종료
#endif
    }
}