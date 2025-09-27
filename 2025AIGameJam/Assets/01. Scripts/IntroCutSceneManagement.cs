using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class IntroCutSceneManagement : MonoBehaviour
{
    [Header("Canvas Refs")]
    public UIManager uiManager;
    public Canvas introCanvas;       // 이 스크립트가 붙은 인트로 캔버스
    public Canvas mainHomeCanvas;    // 전환 대상 메인 홈 캔버스

    [Header("Skip Button")]
    // OnClick에 1) UIManager.LoadCanvas(mainHomeCanvas)
    //          2) (선택) IntroCutSceneManagement.OnSkipFinalize() 를 같이 등록하면 더 안전
    public Button skipButton;

    [Header("Intro Objects")]
    public GameObject scene1Background;

    // TMP는 각 TextBackground(Panel)의 "자식"
    public TextMeshProUGUI scene1Text;
    public TextMeshProUGUI scene2Text;

    public GameObject flashPanel;             // 흰색 Image + CanvasGroup
    public GameObject sceneAttackBackground;
    public GameObject waitPanel;

    public GameObject chunsoon_1;
    public GameObject chunsoon_2;

    public GameObject player_1;
    public GameObject player_2;
    public TextMeshProUGUI playerText;

    [Header("Black Fade Panel")]
    public GameObject fadePanel;              // 검정 Image + CanvasGroup

    [Header("Timings")]
    // 섬광탄 느낌(짧게 확 밝고 빠르게 사라짐)
    public float flashIn   = 0.06f;
    public float flashHold = 0.04f;
    public float flashOut  = 0.20f;
    // 마지막 전환용 검정 페이드 아웃(페이드 인 없음)
    public float finalFadeOut = 0.5f;

    private int  currentStep = 0;
    private bool isAnimating = false;
    private bool shutDown    = false;
    private const int MaxStep = 8;

    // ✅ IntroCanvas가 LoadCanvas로 "켜질 때"마다 초기화
    void OnEnable()
    {
        ResetIntroState();
    }

    // ✅ IntroCanvas가 "꺼질 때" 자동 정리 (메인 홈/타이틀로 전환 시)
    void OnDisable()
    {
        ShutdownIntro();
    }

    void Update()
    {
        if (shutDown || isAnimating) return;

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
                // 마지막 이후 입력 → 검정 페이드 아웃 → 메인 홈 전환 → 즉시 검정 패널 끔
                if (fadePanel != null)
                {
                    StartCoroutine(FadePanelOnce(
                        panel:        fadePanel,
                        inDuration:   finalFadeOut,
                        hold:         0f,
                        outDuration:  0f,         // 페이드 인 없음
                        maxAlpha:     1f,
                        onReachedMax: () =>
                        {
                            uiManager.LoadCanvas(mainHomeCanvas, introCanvas);
                            // 전환 직후 인트로 완전 종료 (입력/코루틴/업데이트 차단)
                            ShutdownIntro();
                        },
                        deactivateAtEnd: true,
                        lockInput:       true
                    ));
                }
                else
                {
                    uiManager.LoadCanvas(mainHomeCanvas, introCanvas);
                    ShutdownIntro();
                }
            }
        }
    }

    void PlayStep(int step)
    {
        switch (step)
        {
            case 1:
                // 인트로 씬 1 텍스트(배경+텍스트) 페이드 인
                isAnimating = true;
                FadeBothIn(scene1Text, 1f, () => isAnimating = false);
                break;

            case 2:
                // 1번 텍스트(배경+텍스트) 페이드 아웃 → 2번 텍스트(배경+텍스트) 페이드 인
                isAnimating = true;
                FadeBothOut(scene1Text, 0.5f, () =>
                {
                    FadeBothIn(scene2Text, 1f, () => isAnimating = false);
                });
                break;

            case 3:
                // 섬광 + 공격당하는 씬 배경 활성화
                StartCoroutine(FadePanelOnce(
                    panel: flashPanel, inDuration: flashIn, hold: flashHold, outDuration: flashOut,
                    maxAlpha: 1f, onReachedMax: null, deactivateAtEnd: true, lockInput: true
                ));
                SafeSetActive(sceneAttackBackground, true);
                break;

            case 4:
                // 섬광 + waitPanel 활성화
                StartCoroutine(FadePanelOnce(
                    panel: flashPanel, inDuration: flashIn, hold: flashHold, outDuration: flashOut,
                    maxAlpha: 1f, onReachedMax: null, deactivateAtEnd: true, lockInput: true
                ));
                SafeSetActive(waitPanel, true);
                break;

            case 5:
                SafeSetActive(chunsoon_1, true);
                break;

            case 6:
                SafeSetActive(chunsoon_2, true);
                break;

            case 7:
                SafeSetActive(player_1, true);
                break;

            case 8:
                SafeSetActive(player_2, true);
                isAnimating = true;
                //FadeBothIn(playerText, 1f, () => isAnimating = false);
                Sequence sequence = DOTween.Sequence();
                sequence.Append(playerText.DOFade(1f, 1));
                sequence.OnComplete(() =>
                {
                    isAnimating = false;
                });
                break;
        }
    }

    // ================== 공용 페이드(검정/섬광) ==================
    System.Collections.IEnumerator FadePanelOnce(
        GameObject panel,
        float inDuration,
        float hold,
        float outDuration,
        float maxAlpha = 1f,
        System.Action onReachedMax = null,
        bool deactivateAtEnd = true,
        bool lockInput = false
    )
    {
        if (panel == null) yield break;

        var cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();

        if (lockInput) isAnimating = true;

        panel.SetActive(true);
        cg.alpha = 0f;

        // 1) 페이드 인(0 → max)
        yield return cg.DOFade(maxAlpha, Mathf.Max(0f, inDuration))
                      .SetEase(Ease.OutExpo)
                      .WaitForCompletion();

        onReachedMax?.Invoke();

        // 2) 유지
        if (hold > 0f) yield return new WaitForSeconds(hold);

        // 3) 페이드 아웃 또는 즉시 종료
        if (outDuration > 0f)
        {
            yield return cg.DOFade(0f, outDuration)
                           .SetEase(Ease.InQuad)
                           .WaitForCompletion();
            if (deactivateAtEnd) panel.SetActive(false);
        }
        else
        {
            if (deactivateAtEnd) panel.SetActive(false);
        }

        if (lockInput) isAnimating = false;
    }

    // =========== 배경(Image) + 텍스트(TMP) 동시 페이드 ===========
    Image GetBgOf(TextMeshProUGUI tmp)
    {
        if (tmp == null || tmp.transform.parent == null) return null;
        return tmp.transform.parent.GetComponent<Image>();
    }

    void InitBothHidden(TextMeshProUGUI tmp)
    {
        if (tmp == null) return;
        var bg = GetBgOf(tmp);

        tmp.alpha = 0f;
        if (bg != null)
        {
            var c = bg.color; c.a = 0f; bg.color = c;
            bg.raycastTarget = false; // 안 보일 때 클릭 통과
        }
    }

    void FadeBothIn(TextMeshProUGUI tmp, float duration, System.Action onComplete = null)
    {
        if (tmp == null) { onComplete?.Invoke(); return; }
        var bg = GetBgOf(tmp);

        var seq = DOTween.Sequence()
                         .Join(tmp.DOFade(1f, duration));

        if (bg != null)
        {
            var c = bg.color; c.a = 0f; bg.color = c;
            bg.raycastTarget = true;            // 보이는 동안 클릭 막기 원하면 true 유지
            seq.Join(bg.DOFade(1f, duration));
        }

        seq.OnComplete(() => onComplete?.Invoke());
    }

    void FadeBothOut(TextMeshProUGUI tmp, float duration, System.Action onComplete = null)
    {
        if (tmp == null) { onComplete?.Invoke(); return; }
        var bg = GetBgOf(tmp);

        var seq = DOTween.Sequence()
                         .Join(tmp.DOFade(0f, duration));

        if (bg != null)
            seq.Join(bg.DOFade(0f, duration));

        seq.OnComplete(() =>
        {
            if (bg != null) bg.raycastTarget = false;
            onComplete?.Invoke();
        });
    }

    // ================== 초기화/종료 & 유틸 ==================
    void ResetIntroState()
    {
        // 인트로는 TitleCanvas의 Start 버튼으로 LoadCanvas 호출 시에만 시작됨
        shutDown     = false;
        isAnimating  = false;
        currentStep  = 0;

        // 네 의도대로: 시작 시 1번 배경은 켜두고, 나머지는 끔
        SafeSetActive(scene1Background, true);

        SafeSetActive(flashPanel, false);
        SafeSetActive(sceneAttackBackground, false);
        SafeSetActive(waitPanel, false);

        SafeSetActive(chunsoon_1, false);
        SafeSetActive(chunsoon_2, false);
        SafeSetActive(player_1, false);
        SafeSetActive(player_2, false);

        // 텍스트/배경 알파 0으로 초기화
        InitBothHidden(scene1Text);
        InitBothHidden(scene2Text);
        InitBothHidden(playerText);

        EnsureCanvasGroup(flashPanel);
        EnsureCanvasGroup(fadePanel);
        if (fadePanel != null) fadePanel.SetActive(false);
    }

    /// <summary>인트로 입력/코루틴 완전 종료</summary>
    void ShutdownIntro()
    {
        if (shutDown) return;
        shutDown    = true;
        isAnimating = false;
        StopAllCoroutines();
        // 이 스크립트는 IntroCanvas가 꺼지면 함께 꺼지므로 enabled=false는 선택
        // enabled = false;
    }

    /// <summary>스킵 버튼 OnClick에 UIManager.LoadCanvas 다음으로 연결하면 더 안전</summary>
    public void OnSkipFinalize()
    {
        ShutdownIntro();
    }

    void EnsureCanvasGroup(GameObject go)
    {
        if (go == null) return;
        if (go.GetComponent<CanvasGroup>() == null) go.AddComponent<CanvasGroup>();
    }

    void SafeSetActive(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }
}
