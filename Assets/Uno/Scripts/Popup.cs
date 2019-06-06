using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Popup : MonoBehaviour
{
    public string popupName;
    public bool isOpen;
    public bool closeOnEsc = true;
    [SerializeField] GameObject maskImage;
    [SerializeField] RectTransform popupRect;
    [SerializeField] float time = .25f;
    public UnityEvent onShow;
    public UnityEvent onHide;
    public static Popup currentPopup;

    void Start()
    {
        if (isOpen)
        {
            ShowPopup();
        }
        else
        {
            HidePopup();
        }
    }

    [ContextMenu("Show")]
    public void ShowPopup(bool invokeOnComplete = true)
    {
        if (LeanTween.isTweening(popupRect) || isOpen)
            return;

        isOpen = true;
        maskImage.gameObject.SetActive(isOpen);
        LeanTween.scale(popupRect, Vector3.one, time).setOnComplete(() =>
        {
            currentPopup = this;
            if (invokeOnComplete)
                onShow.Invoke();
        }).setIgnoreTimeScale(true);
    }

    [ContextMenu("Hide")]
    public void HidePopup(bool invokeOnComplete = true)
    {
        if (LeanTween.isTweening(popupRect) || !isOpen)
        {
            return;
        }
        isOpen = false;
        currentPopup = null;
        maskImage.gameObject.SetActive(isOpen);
        LeanTween.scale(popupRect, Vector3.zero, time).setOnComplete(() =>
        {
            if (invokeOnComplete)
                onHide.Invoke();
        }).setIgnoreTimeScale(true);
    }
}
