using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CarouselManager : MonoBehaviour, IEndDragHandler
{
    [SerializeField]
    private Canvas m_Canvas;
    private ScrollRect m_ScrollRect;
    [SerializeField]
    private RectTransform ContentBox;
    [SerializeField]
    private GameObject[] BannerPrefabs;
    [SerializeField]
    private RectTransform IndicatorsContainer;
    [SerializeField]
    private CarouselIndicator IndicatorPrefab;
    private List<CarouselIndicator> m_Indicators = new();
    [SerializeField]
    private bool AutoScroll = true;
    [SerializeField]
    private float AutoScrollInterval = 5f;
    private float m_AutoScrollTimer;
    [SerializeField, Range(0.25f, 1f)]
    private float ScrollDuration = 0.5f;
    [SerializeField]
    private AnimationCurve EaseCurve;

    private int m_CurrentIndex = 0;
    private Coroutine m_ScrollCoroutine;

    private void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }
    void Start()
    {
        foreach (var bannerPrefab in BannerPrefabs)
        {
            GameObject banner = Instantiate(bannerPrefab, ContentBox);
            RectTransform bannerTransform = banner.GetComponent<RectTransform>();
            if (bannerTransform)
                bannerTransform.sizeDelta = new(Screen.width / m_Canvas.scaleFactor, bannerTransform.sizeDelta.y);
            m_Indicators.Add(Instantiate(IndicatorPrefab, IndicatorsContainer));
        }
        m_Indicators[0].Activate();
        m_AutoScrollTimer = AutoScrollInterval;
    }

    private IEnumerator MoveToPos(float targetPos)
    {
        float elapsedTime = 0f;
        float initialPos = m_ScrollRect.horizontalNormalizedPosition;
        if (ScrollDuration > 0f)
        {
            while (elapsedTime <= ScrollDuration)
            {
                float easeValue = EaseCurve.Evaluate(elapsedTime / ScrollDuration);
                float newPos = Mathf.Lerp(initialPos, targetPos, easeValue);
                m_ScrollRect.horizontalNormalizedPosition = newPos;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        m_ScrollRect.horizontalNormalizedPosition = targetPos;
    }

    private void ScrollTo(int index)
    {
        m_Indicators[m_CurrentIndex].Deactivate();
        m_CurrentIndex = index;
        m_AutoScrollTimer = AutoScrollInterval;
        float targetPos = (float)m_CurrentIndex / (BannerPrefabs.Length - 1);
        if (m_ScrollCoroutine != null)
            StopCoroutine(m_ScrollCoroutine);
        m_ScrollCoroutine = StartCoroutine(MoveToPos(targetPos));
        m_Indicators[m_CurrentIndex].Activate();
    }

    public void ScrollToNext()
    {
        int newIndex = (m_CurrentIndex + 1) % BannerPrefabs.Length;
        ScrollTo(newIndex);
    }

    public void ScrollToPrevious()
    {
        int newIndex = (m_CurrentIndex - 1 + BannerPrefabs.Length) % BannerPrefabs.Length;
        ScrollTo(newIndex);
    }

    private void Update()
    {
        if (!AutoScroll)
            return;
        m_AutoScrollTimer -= Time.deltaTime;
        if (m_AutoScrollTimer <= 0)
            ScrollToNext();
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (data.delta.x > 0)
            ScrollToPrevious();
        else if (data.delta.x < 0)
            ScrollToNext();
        else
            ScrollTo(m_CurrentIndex);
    }
}
