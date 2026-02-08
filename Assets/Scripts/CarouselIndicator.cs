using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CarouselIndicator : MonoBehaviour
{
    private Image m_image;
    [SerializeField]
    private Sprite activeSprite;
    [SerializeField]
    private Sprite inactiveSprite;

    private void Awake()
    {
        m_image = GetComponentInChildren<Image>();
        inactiveSprite = m_image.sprite;
    }

    public void Activate()
    {
        m_image.sprite = activeSprite;
    }

    public void Deactivate()
    {
        m_image.sprite = inactiveSprite;
    }
}
