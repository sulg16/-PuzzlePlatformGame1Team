using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// 게임의 UI 상태를 정의하는 열거형
public enum GameState
{
    Intro,
    Playing,
    Paused,
    GameOver,
}

public partial class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    // GameState 타입을 사용하는 이벤트 함수 OnGameStateChanged를 선언
    public static event Action<GameState> OnGameStateChanged;
    private GameState _currentState;
    bool IsPause = false;

    private bool _timeEventBound = false;

    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject persistentUiPrefab;
    private PersistentUIController ui;


    // 현재 씬을 저장 할 변수 세팅
    private Scene currentScene;

    public static GameManager Instance
    {
        get
        {
            // 할당되지 않았을 때, 외부에서 GameManager.Instance 로 접근하는 경우
            // 게임 오브젝트를 만들어주고 GameManager 스크립트를 AddComponent로 붙여준다.
            if (_instance == null)
            {
                // 게임오브젝트가 없어도 시작시 없는걸 확인후 매니저를 게임오브젝트로 생성해줌
                _instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return _instance;
        }
    }






    public GameState CurrentGameState
    {
        // CurrentGameState 호출한 곳에 _currentState 반환
        get { return _currentState; }
        private set
        {
            // CurrentGameState 호출한 곳에서 넘긴 정보(value)를 _currentState 변수에 저장
            _currentState = value;
            // OnGameStateChanged라는 함수의 구독 내용이 있다면? 전부 이벤트 발생 (Invoke)
            OnGameStateChanged?.Invoke(_currentState);
        }
    }

    // Player 인스턴스에 접근하기 위한 Instance 함수

    private void Awake()
    {
        var direction = DirectionManager.Instance;
        var audio = AudioManager.Instance;


        // Awake가 호출 될 때라면 이미 매니저 오브젝트는 생성되어 있는 것이고, '_instance'에 자신을 할당
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬매니저 씬로드 이벤트에 게임매니저의 OnSceneLoaded 함수를 구독

            EnsurePersistentUI();
        }
        else
        {
            // 이미 오브젝트가 존재하는 경우 '자신'을 파괴해서 중복방지
            if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        // 현재 씬을 가져와서 변수에 저장
        currentScene = SceneManager.GetActiveScene();
    }

    // GameState 전환 함수
    public void ChangeGameState(GameState newState)
    {
        CurrentGameState = newState;
    }

    // 게임 시작
    public void StartGame()
    {
        // SceneManagement를 통해 1번 씬으로 전환
        SceneManager.LoadScene(1);
    }

    // 게임 일시정지
    public void PauseGame()
    {
        // GameState 상태를 Paused로 전환
        ChangeGameState(GameState.Paused);
        Debug.Log($"GameState : {_currentState}");

        // 게임 정지
        if (IsPause == false)
        {
            Time.timeScale = 0;
            IsPause = true;
            return;
        }
    }

    // 게임 일시정지
    public void ReturnGame()
    {
        // GameState 상태를 Playing으로 전환
        ChangeGameState(GameState.Playing);

        // 게임 정지
        if (IsPause == true)
        {
            Time.timeScale = 1;
            IsPause = false;
            return;
        }
    }

    // 게임 오버
    public void GameOver()
    {
        // GameState 상태를 GameOver로 전환
        ChangeGameState(GameState.GameOver);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindTimeEvents();
        EnsurePersistentUI();

        bool isPlayScene = (scene.name == "MainScene_Floor1" || scene.name == "MainScene_Floor2");

        // 플레이 씬 진입: 시간 '유지' → 이미 돌고 있으면 그대로, 아니면 시작
        if (isPlayScene)
        {
            // 이전 씬에서 Pause였을 수 있으니 정상화
            Time.timeScale = 1f;
            IsPause = false;

            ChangeGameState(GameState.Playing);

            WorldTimeManager.Instance.StartIfNotRunning();
        }

        // 엔딩 씬: 시간 정지
        if (scene.name == "EndingScene")
        {
            WorldTimeManager.Instance.StopTime();
        }

        // 이하 기존 씬별 로직 --------------------------------

        if (scene.name == "IntroScene")
        {
            DirectionManager.Instance.Direction_Intro();
        }

        if (scene.name == "MainScene_Floor2")
        {
            DirectionManager.Instance._mainCam.gameObject.SetActive(true);
            Player.Instance._controller.LockOnInput(0);
            Cursor.lockState = CursorLockMode.Locked;

            AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame;
            AudioManager.Instance.bgmSource.Play();
        }
        else if (scene.name == "MainScene_Floor1")
        {
            AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame;
            AudioManager.Instance.bgmSource.Play();

            Debug.Log("좌표변경");
            Player.Instance.transform.position = new Vector3(7, -6, -18);
        }
        else if (scene.name == "EndingScene")
        {
            AudioManager.Instance.bgmSource.clip = AudioManager.Instance.bgmGame2;
            AudioManager.Instance.bgmSource.Play();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (WorldTimeManager.Instance != null)
            WorldTimeManager.Instance.OnTimeExpired -= HandleTimeExpired;

        _timeEventBound = false;
    }


    private void BindTimeEvents()
    {
        if (_timeEventBound) return;
        if (WorldTimeManager.Instance == null) return;

        WorldTimeManager.Instance.OnTimeExpired += HandleTimeExpired;
        _timeEventBound = true;
    }

    private void HandleTimeExpired()
    {
        Debug.Log($"[GameManager] HandleTimeExpired called. state={CurrentGameState}");

        if (CurrentGameState == GameState.GameOver) return;

        GameOver();
        Time.timeScale = 0f;
        IsPause = true;

        // 게임오버 패널 표시(기존 UI 재사용)
        if (UI_Manager.Instance != null && UI_Manager.Instance._gameOver != null)
            UI_Manager.Instance._gameOver.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        var pc = FindObjectOfType<PlayerController>(true);
        if (pc != null) pc.LockOnInput(1);
    }

    private void EnsurePersistentUI()
    {
        if (ui != null) return;

        // 이미 존재하면 재사용
        ui = FindFirstObjectByType<PersistentUIController>();
        if (ui != null) return;

        if (persistentUiPrefab == null)
        {
            Debug.LogError("GameManager에 persistentUiPrefab이 연결되지 않았습니다.");
            return;
        }

        var go = Instantiate(persistentUiPrefab);
        DontDestroyOnLoad(go);

        ui = go.GetComponent<PersistentUIController>();
        if (ui == null)
            Debug.LogError("Persistent UI 프리팹 루트에 PersistentUIController가 없습니다.");
    }
}
