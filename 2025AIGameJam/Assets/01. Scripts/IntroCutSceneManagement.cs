using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class IntroCutSceneManagement : MonoBehaviour
{
    [Header("Canvas Refs")]
    public UIManager uiManager;      // UIManager 참조
    public Canvas introCanvas;       // 현재 인트로 캔버스
    public Canvas mainHomeCanvas;    // 메인 홈 캔버스

    [Header("Skip Button")]
    public Button skipButton;        // 인스펙터에서 OnClick에 UIManager.LoadCanvas(mainHomeCanvas) 등록

    [Header("Intro Objects")]
    public GameObject scene1Background;
    public TextMeshProUGUI scene1Text;
    public TextMeshProUGUI scene2Text;

    public GameObject flashPanel;            // 깜빡임용 (CanvasGroup 필요)
    public GameObject sceneAttackBackground;
    public GameObject waitPanel;

    public GameObject chunsun_1;
    public GameObject chunsun_2;

    public GameObject player_1;
    public GameObject player_2;
    public TextMeshProUGUI playerText;

    [Header("Black Fade Panel")]
    public GameObject fadePanel;             // 전체 화면 검정 패널 (CanvasGroup 필요)

    private int currentStep = 0;
    private bool isAnimating = false;
    private const int MaxStep = 8;           // 마지막 단계 번호
    
    // 인트로 캔버스 켜기
    // 인트로 씬 1번의 배경 켜기
    // 인트로 씬 1번 텍스트 페이드 
    // 2번 텍스트 페이드
    // 섬광 연출
    // 공격당하는 씬 Background 활성화
    // 잠깐! 띄우기
    // 섬광 연출 
    // 천순이 등장 이미지 
    // 천순이 확대 이미지 (설명할 시간 없어 어서타)
    // 아 너무 멋지다 이미지
    // 주인공 클로즈업 이미지 
    // 텍스트 페이드 까짓거 한번 해보죠.

    void Start()
    {
        // 초기 상태 세팅
        introCanvas.gameObject.SetActive(true);
        if (mainHomeCanvas != null) mainHomeCanvas.gameObject.SetActive(false);

        SafeSetActive(scene1Background, false);
        SafeSetAlpha(scene1Text, 0f);
        SafeSetAlpha(scene2Text, 0f);

        SafeSetActive(flashPanel, false);
        SafeSetActive(sceneAttackBackground, false);
        SafeSetActive(waitPanel, false);

        SafeSetActive(chunsun_1, false);
        SafeSetActive(chunsun_2, false);
        SafeSetActive(player_1, false);
        SafeSetActive(player_2, false);
        SafeSetAlpha(playerText, 0f);

        // 패널 CanvasGroup 확보
        EnsureCanvasGroup(flashPanel);
        EnsureCanvasGroup(fadePanel);
        if (fadePanel != null) fadePanel.SetActive(false);
    }

    void Update()
    {
        if (isAnimating) return;

        // 스킵 버튼을 클릭한 프레임은 인트로 진행 입력 무시 (스킵은 OnClick에서 처리)
        if (EventSystem.current != null && skipButton != null)
        {
            var sel = EventSystem.current.currentSelectedGameObject;
            if (sel == skipButton.gameObject) return;
        }

        // 마우스 좌클릭 or 키보드 아무 키
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            if (currentStep < MaxStep)
            {
                currentStep++;
                PlayStep(currentStep);
            }
            else
            {
                // 마지막 이후 입력 → 페이드 아웃 후 메인 홈 전환(페이드 인 없음, 바로 패널 끔)
                StartCoroutine(FadeToMainHome());
            }
        }
    }

    void PlayStep(int step)
    {
        switch (step)
        {
            case 1:
                SafeSetActive(scene1Background, true);
                isAnimating = true;
                scene1Text.DOFade(1f, 1f).OnComplete(() => isAnimating = false);
                break;

            case 2:
                isAnimating = true;
                scene1Text.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    scene2Text.DOFade(1f, 1f).OnComplete(() => isAnimating = false);
                });
                break;

            case 3:
                StartCoroutine(FlashRoutine(3, 0.2f));
                break;

            case 4:
                SafeSetActive(sceneAttackBackground, true);
                SafeSetActive(waitPanel, true);
                StartCoroutine(FlashRoutine(3, 0.2f));
                break;

            case 5:
                SafeSetActive(chunsun_1, true);
                break;

            case 6:
                SafeSetActive(chunsun_2, true);
                break;

            case 7:
                SafeSetActive(player_1, true);
                break;

            case 8:
                SafeSetActive(player_2, true);
                isAnimating = true;
                playerText.DOFade(1f, 1f).OnComplete(() => isAnimating = false);
                break;

            default:
                break;
        }
    }

    System.Collections.IEnumerator FlashRoutine(int times, float eachFade)
    {
        isAnimating = true;
        if (flashPanel == null) { isAnimating = false; yield break; }

        var cg = flashPanel.GetComponent<CanvasGroup>();
        flashPanel.SetActive(true);

        for (int i = 0; i < times; i++)
        {
            cg.alpha = 0f;
            yield return cg.DOFade(1f, eachFade).WaitForCompletion();
            yield return cg.DOFade(0f, eachFade).WaitForCompletion();
        }

        flashPanel.SetActive(false);
        isAnimating = false;
    }

    System.Collections.IEnumerator FadeToMainHome()
    {
        isAnimating = true;

        if (fadePanel == null)
        {
            // 예비 처리: 패널 없으면 바로 전환
            uiManager.LoadCanvas(mainHomeCanvas, introCanvas);
            isAnimating = false;
            yield break;
        }

        var cg = fadePanel.GetComponent<CanvasGroup>();
        fadePanel.SetActive(true);
        cg.alpha = 0f;

        // 검은 화면으로 페이드 아웃
        yield return cg.DOFade(1f, 0.5f).WaitForCompletion();

        // 메인 홈으로 전환
        uiManager.LoadCanvas(mainHomeCanvas, introCanvas);

        // 바로 패널 비활성화 → 메인 홈 UI가 즉시 보임
        fadePanel.SetActive(false);

        isAnimating = false;
    }

    // -------- 유틸 --------
    void SafeSetActive(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }

    void SafeSetAlpha(TextMeshProUGUI tmp, float a)
    {
        if (tmp != null) tmp.alpha = a;
    }

    void EnsureCanvasGroup(GameObject go)
    {
        if (go == null) return;
        if (go.GetComponent<CanvasGroup>() == null) go.AddComponent<CanvasGroup>();
    }
}