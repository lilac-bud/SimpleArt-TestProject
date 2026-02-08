using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    public static GalleryManager Instance { get; private set; }

    private ScrollRect m_ScrollRect;
    [SerializeField]
    private int ImageMaxCount = 66;
    [SerializeField]
    private int PremiumInterval = 4;
    private Sprite[] m_Images;
    [SerializeField]
    private RectTransform PreviewsBox;
    [SerializeField]
    private GalleryPreview PreviewPrefab;
    private List<GalleryPreview> m_Previews = new();
    [SerializeField]
    private GameObject LoadLine;
    [SerializeField]
    private string BaseURL = "http://data.ikppbb.com/test-task-unity-data/pics/";

    public GameObject ViewImagePopup;
    public Image ViewImagePopupContent;
    public GameObject PremiumPopup;

    private Coroutine m_LoadPreviewsCoroutine;

    private enum Tabs { All, Odd, Even };

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        m_ScrollRect = GetComponent<ScrollRect>();
    }

    void Start()
    {
        m_Images = new Sprite[ImageMaxCount];
        LoadTab(Tabs.All);
    }

    private void ClearPreviews()
    {
        if (m_Previews.Count > 0)
        {
            foreach (var preview in m_Previews)
                Destroy(preview.gameObject);
            m_Previews.Clear();
        }
    }

    private void LoadTab(Tabs tab)
    {
        ClearPreviews();
        m_ScrollRect.verticalNormalizedPosition = 1f;
        for (int i = 0, premiumCount = 0; i < ImageMaxCount; i++)
        {
            switch (tab)
            {
                case Tabs.All:
                    break;
                case Tabs.Odd:
                    if ((i + 1) % 2 == 0)
                        continue;
                    break;
                case Tabs.Even:
                    if ((i + 1) % 2 != 0)
                        continue;
                    break;
                default:
                    break;
            }
            GalleryPreview newPreview = Instantiate(PreviewPrefab, PreviewsBox);
            newPreview.PreviewID = i;
            premiumCount++;
            if (premiumCount == PremiumInterval)
            {
                newPreview.MakePremium();
                premiumCount = 0;
            }
            m_Previews.Add(newPreview);
        }
        if (m_LoadPreviewsCoroutine != null)
            StopCoroutine(m_LoadPreviewsCoroutine);
        m_LoadPreviewsCoroutine = StartCoroutine(LoadPreviews());
    }

    private IEnumerator LoadPreviews()
    {
        foreach (var preview in m_Previews)
        {
            if (m_Images[preview.PreviewID])
                preview.SetImage(m_Images[preview.PreviewID]);
            else
            {
                yield return new WaitUntil(() => preview.transform.position.y > LoadLine.transform.position.y);
                string requestURL = BaseURL + (preview.PreviewID + 1) + ".jpg";
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(requestURL);
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Texture2D requestedTexture = DownloadHandlerTexture.GetContent(www);
                    Sprite imageSprite = Sprite.Create(requestedTexture,
                        new Rect(0.0f, 0.0f, requestedTexture.width, requestedTexture.height), new Vector2(0.5f, 0.5f), 100f);
                    m_Images[preview.PreviewID] = imageSprite;
                    preview.SetImage(imageSprite);
                }
            }
        }
    }

    public void ChangeTabToAll(bool change)
    {
        if (change)
            LoadTab(Tabs.All);
    }
    public void ChangeTabToOdd(bool change)
    {
        if (change)
            LoadTab(Tabs.Odd);
    }
    public void ChangeTabToEven(bool change)
    {
        if (change)
            LoadTab(Tabs.Even);
    }
}
