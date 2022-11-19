using System;
using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public static event Action StageStatusWhenContinueNextStage;

    public UIManager UıManager => _uiManager;
    public int LevelCount => _levelCount;
    public int StageCount => _stageCount;
    
    private LevelScriptable[] _levelData;
    private int _levelCount;
    private int _stageCount;
    private int _barCount;
    private int _ballCountInsidePool;
    private Transform _spawnPoint;
    private Transform _startPoint;
    private GameObject[] _movingPool;
    private GameObject[] _floors;
    private TMP_Text[] _ballInsidePoolText;
    
    
    [SerializeField] private Transform firstSpawnPoint;
    [SerializeField] private GameObject _levelContainer;
    [SerializeField] private GameObject _fakeEnvironment;

    private UIManager _uiManager;
    private PlayerController _playerController;
    

    private string _saveData;

    void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
        
        _uiManager = GetComponent<UIManager>();

        LoadLevelData();
        LoadAssets();

        //UI FUNCS
        
        _uiManager.SetHud(false);
        _uiManager.UpdateLevelUI();
        _uiManager.SetStageUI();
        _uiManager.ScaleTapButton();

    }

    private void LoadAssets()
    {
        _fakeEnvironment.gameObject.SetActive(true);
        _levelData = Resources.LoadAll<LevelScriptable>("Scriptable Objects/Level Scriptables");
        _ballInsidePoolText = new TMP_Text[3];
        _movingPool = new GameObject[3];
        _floors = new GameObject[3];
    }

    void Start()
    {
        GameManager.Instance.GameInitialized += OnGameInitialized;
        _playerController = GameManager.Instance.PlayerController;
    }
    private void OnGameInitialized()
    {
        _fakeEnvironment.gameObject.SetActive(false);
    }

    #region Bar Settings
    public void FillBarCount()
    {
        _barCount++;
    }

    public void ResetBarCount()
    {
        _barCount = 0;
    }
    
    #endregion

    #region Level Init Settings
    public void CreateNextLevel()
    {
        GameObject level = Instantiate(_levelData[_levelCount-1].LevelPrefab, _spawnPoint.position,transform.rotation);
        level.transform.parent = _levelContainer.transform;
        Destroy(_levelContainer.transform.GetChild(0).gameObject,10f);
        _playerController.HasInput = true;
        _playerController.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void CreateFirstLevel()
    {
        GameObject level = Instantiate(_levelData[_levelCount-1].LevelPrefab, firstSpawnPoint.position, transform.rotation);
        level.transform.parent = _levelContainer.transform;

    }

    public void SetStageRequiredBallCountText(TMP_Text[] poolsRequiredBallText)
    {
        for (int i = 0; i < 3; i++)
        {
            poolsRequiredBallText[i].text = "/ " + _levelData[_levelCount-1].PoolsRequiredBallCount[i].ToString();
        }
    }

    public void SetMovingPoolGameObject(GameObject[] movingPool)
    {
        for (int i = 0; i < 3; i++)
        {
            _movingPool[i] = movingPool[i];
        }
    }

    public void SetStageFillBallCountText(TMP_Text[] ballInsidePoolText)
    {
        for (int i = 0; i < 3; i++)
        {
            _ballInsidePoolText[i] = ballInsidePoolText[i];
            _ballInsidePoolText[i].text = "0";
        }
    }

    public void SetFloorColor(GameObject[] floors, Color floorColor)
    {
        for (int i = 0; i < 3; i++)
        {
            _floors[i] = floors[i];
            _floors[i].GetComponent<MeshRenderer>().material.color = floorColor;
        }
    }

    public void SetSpawnPoint(Transform spawnPoint)
    {
        _spawnPoint = spawnPoint;
    }

    public void SetStartPoint(Transform startPoint)
    {
        _startPoint = startPoint;
    }

    #endregion

    #region Level, Stage, Pool Settings
    
    
    public void PassNextStage()
    {
        UıManager.SetStageUI();
        StageStatusWhenContinueNextStage += IncreaseStageCount;
        StageStatusWhenContinueNextStage += ResetCurrentBallCountInsidePool;
        StageStatusWhenContinueNextStage?.Invoke();
        StageStatusWhenContinueNextStage = null;
    }

    public void IncreaseCurrentBallCountInsidePool()
    {
        _ballCountInsidePool++;
    }

    public void UpdateCurrentBallCountTextInsidePool()
    {
        _ballInsidePoolText[_stageCount].text = _ballCountInsidePool.ToString();

    }
    public void SetPlayerPositionToStartPoint(Transform player)
    {
        player.DOMoveX(0f, 2.5f);
        player.DOMoveZ(_startPoint.position.z, 2.5f).SetEase(Ease.InOutSine);
    }

    private void ResetCurrentBallCountInsidePool()
    {
        _ballCountInsidePool = 0;
    }

    public void SetEnableMovingPool()
    {
        _movingPool[_stageCount].SetActive(true);
        _movingPool[_stageCount].GetComponent<MeshRenderer>().material.color = _floors[0].GetComponent<MeshRenderer>().material.color;
    }

    private void IncreaseStageCount()
    {
        _stageCount++;
        if (_stageCount == 3)
            _stageCount = 0;
        _uiManager.SetStageUI();
    }

    public void IncreaseLevelCount()
    {
        if(_levelCount<_levelData.Length)
        {
            _levelCount++;
            _uiManager.UpdateLevelUI();
            SaveLevelData();
        }
        else
        {
            _levelCount = 1;
            _uiManager.UpdateLevelUI();
            SaveLevelData();
        }
        _uiManager.ResetStageUI();

    }
    

    public bool CheckPoolHaveRequiredBall()
    {
        if (_ballCountInsidePool >= _levelData[_levelCount-1].PoolsRequiredBallCount[_stageCount])
        {
            return true;
        }
        else
            return false;
    }

    private void SaveLevelData()
    {
        PlayerPrefs.SetInt(_saveData, _levelCount);
        PlayerPrefs.Save();
    }

    private void LoadLevelData()
    {
        if (!PlayerPrefs.HasKey(_saveData))
            PlayerPrefs.SetInt(_saveData, 1);
        _levelCount = PlayerPrefs.GetInt(_saveData);
    }
    #endregion

    
}
