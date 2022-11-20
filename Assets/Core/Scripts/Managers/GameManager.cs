using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsBallsFallInPool { get; private set; }
    public bool IsNewLevelCreated { get; private set; }

    [SerializeField] private Transform _player;
    
    public static event Action StageStatus_When_BallsFallInPool;
    public static event Action ParkourStatus_When_LevelEnding;
    public event Action GameInitialized;

    public PlayerController PlayerController => _playerController;
    
    private UIManager _uiManager;
    private PlayerController _playerController;
    public BridgeController CurrentBridge;

    void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
        
        _playerController = _player.GetComponent<PlayerController>();

        GameInitialized += SetLevelFirstStart;
    }

    void Start()
    {
        _uiManager = LevelManager.Instance.UıManager;
    }


    #region Balls Fall In Pool Settings

    public void SetBallsFallInPool()
    {
        StageStatus_When_BallsFallInPool = SetIsBallsFallInPoolTrue;
        StageStatus_When_BallsFallInPool.Invoke();
    }

    private void ContinueMovingAfterBallFallInPool()
    {
        StageStatus_When_BallsFallInPool = SetIsBallsFallInPoolFalse;
        StageStatus_When_BallsFallInPool.Invoke();
    }

    
    private void SetIsBallsFallInPoolTrue()
    {
        IsBallsFallInPool = true;
    }

    private void SetIsBallsFallInPoolFalse()
    {
        IsBallsFallInPool = false;
    }

    #endregion

    #region Finish Parkour Settings

    private void SetPlayerStartParkour()
    {
        ParkourStatus_When_LevelEnding = SetIsPlayerStartParkourTrue;
        ParkourStatus_When_LevelEnding.Invoke();
        StartCoroutine(SetPlayerFinishPushingParkour());
    }

    private void SetIsPlayerStartParkourTrue()
    {
    }

    private void SetIsPlayerStartParkourFalse()
    {
    }

    IEnumerator SetPlayerFinishPushingParkour()
    {
        yield return new WaitForSeconds(.5f);
        ParkourStatus_When_LevelEnding = SetIsPlayerStartParkourFalse;
        ParkourStatus_When_LevelEnding.Invoke();
    }

    #endregion

    #region New Level Settings

    public void SetGameNewLevel()
    {
        _uiManager.OpenLevelWin();
    }

    public void OnNextLevelClick()
    {
        SetPlayerStartParkour();
        LevelManager.Instance.IncreaseLevelCount();
        LevelManager.Instance.CreateNextLevel();
        IsNewLevelCreated = true;
        StartCoroutine(SetPlayerPosition());
        StartCoroutine(SetNewLevelCreatingFalse());
        _uiManager.ResetLevelEndUI();
    }

    IEnumerator SetPlayerPosition()
    {
        yield return new WaitForSeconds(0.1f);
        LevelManager.Instance.SetPlayerPositionToStartPoint(_player);
        IsNewLevelCreated = false;
    }

    IEnumerator SetNewLevelCreatingFalse()
    {
        yield return new WaitForSeconds(5f);
        IsNewLevelCreated = false;
    }

    #endregion

    #region First Start Level Settings

    private void SetLevelFirstStart()
    {
        LevelManager.Instance.CreateFirstLevel();
        _uiManager.UpdateLevelUI();
    }

    #endregion


    public void SetStageStatusToBallFallInsidePool()
    {
        StartCoroutine(CheckBallInsidePoolSeq());
    }

    IEnumerator CheckBallInsidePoolSeq()
    {
        int timer = 0;
        int timerMax = 6;
        while (timer < timerMax)
        {
            yield return new WaitForSeconds(.5f);
            LevelManager.Instance.UpdateCurrentBallCountTextInsidePool();
            timer++;
            if (timer == timerMax)
            {
                bool isPoolHaveRequiredBall = LevelManager.Instance.CheckPoolHaveRequiredBall();
                if (isPoolHaveRequiredBall)
                {
                    PassNextStage();
                }
                else
                {
                    _uiManager.OpenLevelFail();
                    Debug.Log("Game Over");
                }
            }
        }
    }

    private void PassNextStage()
    {
        StartCoroutine(NextStageCoroutine());
    }

    private IEnumerator NextStageCoroutine()
    {
        CurrentBridge.OpenBridge();
        LevelManager.Instance.SetEnableMovingPool();
        yield return new WaitForSeconds(1.5f);
        ContinueMovingAfterBallFallInPool();
        LevelManager.Instance.PassNextStage();
        _playerController.SetPlayerState(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        _playerController.ResetScale();
        _uiManager.ResetLevelEndUI();
    }

    public void OnGameInitialized()
    {
        GameInitialized?.Invoke();
    }
}