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
    [SerializeField] private TMP_Text currentLevelText;
    [SerializeField] private TMP_Text nextLevelText;

    [SerializeField] private GameObject _hudParent;
    
    [SerializeField] private Image _stage1Image;
    [SerializeField] private Image _stage2Image;
    [SerializeField] private Image _stage3Image;


    [SerializeField] private GameObject _levelFail;
    [SerializeField] private GameObject _levelWin;
    private Tween tapLoop;

    
    void Start()
    {
        GameManager.Instance.GameInitialized += OnGameInitialized;

    }
    private void OnGameInitialized()
    {
        _tapImage.gameObject.SetActive(false);
        _background.gameObject.SetActive(false);
        tapLoop.Kill();
        SetHud(true);
        ResetStageUI();
        SetStageUI();
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

    }

    public void ScaleTapButton()
    {
        Vector3 defaultScale = _tapImage.transform.localScale;
        Vector3 targetScale = Vector3.one * .6f;
        tapLoop = _tapImage.transform.DOScale(targetScale, .5f).OnComplete(() =>
        {
            _tapImage.transform.DOScale(defaultScale,.5f);
        });
        tapLoop.SetLoops(-1, LoopType.Yoyo);
    }

    public void UpdateLevelUI()
    {
        currentLevelText.text = LevelManager.Instance.LevelCount.ToString();
        nextLevelText.text = (LevelManager.Instance.LevelCount + 1).ToString();
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
