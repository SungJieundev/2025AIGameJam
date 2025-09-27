using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public void LoadCanvas(Canvas targetCanvas)
    {
        // 1. 새로 띄울 캔버스 활성화
        targetCanvas.gameObject.SetActive(true);

        // 2. 현재 클릭된 버튼이 속한 캔버스를 비활성화
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        if (clickedButton != null)
        {
            Canvas parentCanvas = clickedButton.GetComponentInParent<Canvas>();
            if (parentCanvas != null && parentCanvas != targetCanvas)
            {
                parentCanvas.gameObject.SetActive(false);
            }
        }
    }

    public void PopupPanel(GameObject targetPanel)
    {
        Sequence sequence = DOTween.Sequence();

        targetPanel.SetActive(true);

        sequence.Append(targetPanel.transform.DOScale(1.2f, 0.15f));
        sequence.Append(targetPanel.transform.DOScale(1f, 0.1f));
    }

    public void PopdownPanel(GameObject targetPanel)
    {
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(targetPanel.transform.DOScale(1.2f, 0.15f));
        sequence.Append(targetPanel.transform.DOScale(1f, 0.1f));
        sequence.OnComplete(() => {
            targetPanel.SetActive(false);
        });
    }

    

    [Header("Game Tips Settings")]
    public List<string> gameTips;            // 게임 팁 리스트
    public TextMeshProUGUI tipText;          // 출력할 TMP 텍스트
    public float fadeDuration = 0.5f;        // 페이드 시간

    private int currentTipIndex = 0;
    private Tween currentTween;

    void Start()
    {
        if (gameTips.Count > 0 && tipText != null)
        {
            // 초기 텍스트 출력
            tipText.text = gameTips[currentTipIndex];
            tipText.alpha = 1f;
        }
    }

    /// <summary>
    /// 👉 버튼 OnClick 이벤트에 이 함수를 등록하세요.
    /// 클릭할 때마다 다음 팁으로 순환하며 전환됩니다.
    /// </summary>
    public void ShowNextTip()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // 다음 인덱스로 이동 (끝나면 다시 0으로 순환)
        currentTipIndex = (currentTipIndex + 1) % gameTips.Count;

        // 페이드 아웃 → 텍스트 교체 → 페이드 인
        Sequence seq = DOTween.Sequence();
        seq.Append(tipText.DOFade(0f, fadeDuration))
            .AppendCallback(() =>
            {
                tipText.text = gameTips[currentTipIndex];
            })
            .Append(tipText.DOFade(1f, fadeDuration));

        currentTween = seq;
    }
    
}
    
    