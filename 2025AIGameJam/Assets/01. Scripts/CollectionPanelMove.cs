using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CollectionPanelMove : MonoBehaviour
{
    //CollectionPanelParent에 들어갈 스크립트

    private int _panelMoveMaxIndex = 1;
    public int currentMoveIndex = 0;
    private int _panelMoveDistance = 1920;

    public GameObject nextButton;
    public GameObject previousButton;
    
    public RectTransform _panelParent;

    

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _panelParent.anchoredPosition = new Vector2(0, 0);
        previousButton.SetActive(false);
        currentMoveIndex = 0;
    }

    public void NextButtonClick()
    {
        //효과음 재생;
        MoveNextPanel();
    }

    public void PreviousButtonClick()
    {
        //효과음 재생
        MovePreviousPanel();
    }

    private void MoveNextPanel()
    {
        _panelParent.DOAnchorPosX(_panelParent.anchoredPosition.x - _panelMoveDistance, 0.5f).SetEase(Ease.OutExpo);
        
        currentMoveIndex++;
        if (currentMoveIndex > 0) previousButton.SetActive(true);
        if(currentMoveIndex == _panelMoveMaxIndex) nextButton.SetActive(false);
        //if(currentMoveIndex == _panelMoveMaxIndex) 
        
        //_panelParent.anchoredPosition = new Vector2(_panelParent.anchoredPosition.x - _panelMoveDistance, 0);
    }

    private void MovePreviousPanel()
    {
        _panelParent.DOAnchorPosX(_panelParent.anchoredPosition.x + _panelMoveDistance, 0.5f).SetEase(Ease.OutExpo);
        
        currentMoveIndex--;
        if (currentMoveIndex == 0) previousButton.SetActive(false);
        if(currentMoveIndex < _panelMoveMaxIndex) nextButton.SetActive(true);
    }
}
