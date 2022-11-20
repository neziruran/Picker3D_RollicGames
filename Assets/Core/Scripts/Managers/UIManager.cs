using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
        
    [Header("UI ELEMENTS")]
    
    [SerializeField] private Image _background;
    [SerializeField] private Image _tapImage;
    [SerializeField] private TMP_Text _currentLevelText;
    [SerializeField] private TMP_Text _nextLevelText;
    [SerializeField] private TMP_Text _scaleUpText;
    [SerializeField] private GameObject _hudParent;
    
    [SerializeField] private Image _stage1Image;
    [SerializeField] private Image _stage2Image;
    [SerializeField] private Image _stage3Image;


    [SerializeField] private GameObject _levelFail;
    [SerializeField] private GameObject _levelWin;
    
    private Tween _scaleTextLoop;
    private Tween _tapImageLoop;

    
    void Start()
    {
        GameManager.Instance.GameInitialized += OnGameInitialized;

    }
    private void OnGameInitialized()
    {
        _tapImage.gameObject.SetActive(false);
        _background.gameObject.SetActive(false);
        _tapImageLoop.Kill();
        SetHud(true);
        ResetStageUI();
        SetStageUI();
    }

    public void OnScaleUp()
    {
        _scaleUpText.gameObject.SetActive(true);
        Vector3 defaultScale = Vector3.one * 1f; // default scale;
        Vector3 targetScale = Vector3.one * 1.25f; // default scale;

        _scaleTextLoop = _scaleUpText.transform.DOScale(targetScale, .5f).OnComplete(() =>
        {
            _scaleUpText.transform.DOScale(defaultScale, .5f);
        });
        _scaleTextLoop.SetLoops(5, LoopType.Yoyo).OnComplete(()=>
        {
            _scaleTextLoop.Kill();
            _scaleUpText.gameObject.SetActive(false);

        });
        

    }

    public void OpenLevelWin()
    {
        _levelWin.SetActive(true);
        
    }

    public void OpenLevelFail()
    {
        _levelFail.SetActive(true);
    }

    public void ResetLevelEndUI()
    {
        _levelWin.SetActive(false);
        _levelFail.SetActive(false);
    }
    
    public void SetHud(bool key)
    {
        _hudParent.SetActive(key);
    }
    public void ResetStageUI()
    {
        DisableStageUi(_stage1Image);
        DisableStageUi(_stage2Image);
        DisableStageUi(_stage3Image);
        _scaleUpText.transform.localScale = Vector3.one;

    }

    public void ScaleTapButton()
    {
        Vector3 defaultScale = _tapImage.transform.localScale;
        Vector3 targetScale = Vector3.one * .6f;
        _tapImageLoop = _tapImage.transform.DOScale(targetScale, .5f).OnComplete(() =>
        {
            _tapImage.transform.DOScale(defaultScale,.5f);
        });
        _tapImageLoop.SetLoops(-1, LoopType.Yoyo);
    }

    public void UpdateLevelUI()
    {
        _currentLevelText.text = LevelManager.Instance.LevelCount.ToString();
        _nextLevelText.text = (LevelManager.Instance.LevelCount + 1).ToString();
        ResetStageUI();
    }
    
    public void SetStageUI()
    {
        switch (LevelManager.Instance.StageCount)
        {
            case 1:
                EnableStageUi(_stage1Image);
                break;
            case 2:
                EnableStageUi(_stage2Image);
                break;
            case 3:
                EnableStageUi(_stage3Image);
                break;
            
        }
    }
    private void EnableStageUi(Image image)
    {
        image.color = Color.green;
    }

    private void DisableStageUi(Image image)
    {
        image.color = Color.white;
    }


}
