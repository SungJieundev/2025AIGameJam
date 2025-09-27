using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CollectionDetailManager : MonoBehaviour
{
    
    //스크립트와 별개로, 모든 메카컬렉션 버튼 OnClick()에 LoadCanvas(메카디테일캔버스)
    // 모든 몬스터컬렉션 버튼 OnClick()에 LoadCanvas(몬스터디테일캔버스)
    
    public float xDuration = 0.1f;
    public float yDuration = 0.5f;
    
    private int _panelMoveDistance = 1920;
    
    [Header("메카 패널")]
    public RectTransform mechaContentContainer;
    public float mechaPanelMinPosX = 0;
    public float mechaPanelMaxPosX = 0;
    public float currentMechaPosX = 0;
    
    public GameObject mechaNextButton;
    public GameObject mechaPreviousButton;
    
    [Header("몬스터 패널")]
    public RectTransform monsterContentContainer;
    public float monsterPanelMinPosX = 0;
    public float monsterPanelMaxPosX = 0;
    public float currentMonsterPosX = 0;
    
    public GameObject monsterNextButton;
    public GameObject monsterPreviousButton;
    
    //온클릭 이벤트에서 호출할 함수 인자로 인덱스를 전달
    
    //메카 도감 버튼 클릭시 호출
    public void CallMechaCollectionDetail(int index)
    {
        mechaContentContainer.anchoredPosition = new Vector2(0, -1080);
        
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(mechaContentContainer.DOAnchorPosX(-1920 * index, xDuration));
        sequence.Append(mechaContentContainer.DOAnchorPosY(0, yDuration).SetEase(Ease.OutExpo));
        sequence.OnComplete(() =>
        {
            currentMechaPosX = mechaContentContainer.anchoredPosition.x;
        });
    }

    //몬스터 도감 버튼 클릭시 호출
    public void CallMonsterCollectionDetail(int index)
    {
        monsterContentContainer.anchoredPosition = new Vector2(0, -1080);
        
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(monsterContentContainer.DOAnchorPosX(-1920 * index, xDuration));
        sequence.Append(monsterContentContainer.DOAnchorPosY(0, yDuration).SetEase(Ease.OutExpo));
        sequence.OnComplete(() =>
        {
            currentMonsterPosX = monsterContentContainer.anchoredPosition.x;
        });
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        //_panelParent.anchoredPosition = new Vector2(0, 0);
        //previousButton.SetActive(false);
        //currentMoveIndex = 0;
    }
    
    //메카

    public void MechaNextButtonClick()
    {
        //효과음 재생;
        MechaMoveNextPanel();
    }

    public void MechaPreviousButtonClick()
    {
        //효과음 재생
        MechaMovePreviousPanel();
    }
    
    
    //확인해야함
    private void MechaMoveNextPanel()
    {
        mechaContentContainer.DOAnchorPosX(mechaContentContainer.anchoredPosition.x - _panelMoveDistance, 0.5f).SetEase(Ease.OutExpo);
        currentMechaPosX = mechaContentContainer.anchoredPosition.x;
        
        if (currentMechaPosX <= mechaPanelMinPosX) mechaPreviousButton.SetActive(true);
        if(currentMechaPosX >= mechaPanelMaxPosX) mechaNextButton.SetActive(false);
    }

    private void MechaMovePreviousPanel()
    {
        mechaContentContainer.DOAnchorPosX(mechaContentContainer.anchoredPosition.x + _panelMoveDistance, 0.5f).SetEase(Ease.OutExpo);
        currentMechaPosX = mechaContentContainer.anchoredPosition.x;
        
        if (currentMechaPosX <= mechaPanelMinPosX) mechaPreviousButton.SetActive(true);
        if(currentMechaPosX >= mechaPanelMaxPosX) mechaNextButton.SetActive(false);
    }
    
    
    // 몬스터
    
    public void MonsterNextButtonClick()
    {
        //효과음 재생;
        MonsterMoveNextPanel();
    }

    public void MonsterPreviousButtonClick()
    {
        //효과음 재생
        MonsterMovePreviousPanel();
    }
    
    private void MonsterMoveNextPanel()
    {
        
        
    }

    private void MonsterMovePreviousPanel()
    {
        
    }

    
    
    
    
    
    
    //스크립트
    ////누를 버튼을 담은 각각의 public List가 존재 (MechaButtonList, MonsterButtonList)
    
    // 캔버스에 배치된 순서대로 각각 버튼을 리스트에 담기
    
    //리스트 중 몇 번 버튼이 눌렸는지 알아내서
    //ContentContainer의 xPos = -1920 * 인덱스번호 
    //ypos를 0으로 (패널이 올라오는 연출)
    
    //NextButtonClick
    //PreviousButtonClick
    
    
    
}
