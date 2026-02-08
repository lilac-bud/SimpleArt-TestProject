using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryPreview : MonoBehaviour
{
    public int PreviewID;
    [SerializeField]
    private Image PreviewImage;
    [SerializeField]
    private GameObject PremiumBadge;
    [SerializeField]
    private Button PreviewButton;

    public void SetImage(Sprite image)
    {
        PreviewImage.sprite = image;
        if (!PremiumBadge.activeSelf)
            PreviewButton.onClick.AddListener(BringUpViewImagePopup);
    }
    public void MakePremium()
    {
        PremiumBadge.SetActive(true);
        PreviewButton.onClick.AddListener(BringUpPremiumPopup);
    }

    private void BringUpViewImagePopup()
    {
        GalleryManager.Instance.ViewImagePopupContent.sprite = PreviewImage.sprite;
        GalleryManager.Instance.ViewImagePopup.SetActive(true);
    }
    private void BringUpPremiumPopup()
    {
        GalleryManager.Instance.PremiumPopup.SetActive(true);
    }
}
