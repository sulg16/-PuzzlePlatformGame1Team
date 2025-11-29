using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

public interface IDamageble //���ݹ������ִ� ��ü�� ���
{
    void TakePhysicalDamage(int damage);
}

public enum PlayerState //�÷��̾��� �������
{
    Idle,
    Run,
    Jump,
    Attack,
    Damaged,
    Dead
}

public class UI_Manager : MonoBehaviour //�����Ͷ� ���� ������
{
    public GameObject toReturnbt;

    public void OnToReturnBtOPEN()
    {
        toReturnbt.gameObject.SetActive(true);
    }

    public void OnToReturnBtClose()
    {
        toReturnbt.gameObject.SetActive(false);
    }

    private static UI_Manager _instance;
    public static UI_Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UI_Manager>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("UI_Manager");
                    _instance = go.AddComponent<UI_Manager>();
                }
            }
            return _instance;
        }
    }

    public PlayerModel _model { get; private set; }
    public PlayerViewModel _viewModel { get; private set; }

    public InventoryModel _model2 { get; private set; }
    public InventoryViewModel _viewModel2 { get; private set; }

    public PlayerView _view;
    public InventoryView _inventory;


    [Header("Panel")]
    public UI_SettingPanel _settingPanel;
    public GameObject _pausePanel;
    public GameObject _gameOver;
    public GameObject _save;
    public GameObject _start;
    [Header("GUI")]
    public GameObject _damageIndigator;
    public GameObject _promptText;
    public UI_ActionKey _uiaction;
    public GameObject _crosshair;
    public GameObject _conditions;
    public GameObject _quickslot;
    public GameObject _pauseButton;
    public GameObject _equipment;



    ItemData _data => ScriptableObject.CreateInstance<ItemData>();

    void Awake()
    {
        Debug.Log("UI_Manager Awake in scene: " + gameObject.scene.name);
        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate UIManager found, destroying this one: " + gameObject.name);
            transform.SetParent(null); // �θ�(Canvas)���� �и�
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _model = new PlayerModel();
        _viewModel = new PlayerViewModel(_model);

        _model2 = new InventoryModel(24);
        _viewModel2 = new InventoryViewModel(_model2);
    }

    private void Start()
    {
        _crosshair.SetActive(false);
        _promptText.SetActive(false);
        _damageIndigator.SetActive(false);
        _uiaction.gameObject.SetActive(false);
        _settingPanel.gameObject.SetActive(false);
        _inventory.gameObject.SetActive(false);
        _quickslot.SetActive(false);
        _pausePanel.SetActive(false);
        _gameOver.SetActive(false);
        _save.SetActive(false);
        _equipment.SetActive(true);
        _conditions.SetActive(true);
        _start.SetActive(true);
        _pauseButton.SetActive(true);

        _settingPanel.InitPanel();

        _inventory.gameObject.SetActive(true);       // �ϴ� Ȱ��ȭ
        _inventory.Init(_viewModel2);               // ���� ���� + ���ε�
        _inventory.gameObject.SetActive(false);     // �ٽ� ����

        _view.Init(_viewModel);
        BindDeathOnce();
    }

    void BindDeathOnce()
    {
        // HP�� 0 ���ϰ� �Ǵ� 'ù ����'���� �ߵ�
        _viewModel.Health
            .Where(h => h <= 0)
            .Take(1)
            .Subscribe(_ =>
            {
                _gameOver.SetActive(true);        // �г� ǥ��
                Time.timeScale = 0f;              // �Ͻ�����
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // (����) �Է� ����
                var pc = FindObjectOfType<PlayerController>(true);
                if (pc != null) pc.LockOnInput(1);
            })
            .AddTo(this); // UI_Manager�� MonoBehaviour��� OK
    }
}