using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    
    public List<string> characterDialogList = new List<string>();
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

    private void DialogPopup()
    {
        
    }
    
}
    
    