using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    public static GameScene instance;
    public Popup menuPopup, exitPopup;
    public Toggle menuSfxToggle;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        menuSfxToggle.isOn = GameManager.IsSound;
        menuSfxToggle.onValueChanged.RemoveAllListeners();
        menuSfxToggle.onValueChanged.AddListener((arg0) =>
        {
            GameManager.PlayButton();
            GameManager.IsSound = arg0;
        });
    }

    private void Start()
    {
        CUtils.ShowInterstitialAd();
        CUtils.CloseBannerAd();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 1f)
        {
            if (Popup.currentPopup != null && Popup.currentPopup.closeOnEsc)
            {
                Popup.currentPopup.HidePopup();
            }
            else if (Popup.currentPopup == null)
            {
                ShowExit();
            }
        }
    }

    public void ShowMenu()
    {
        menuPopup.ShowPopup();

        Timer.Schedule(this, 0.25f, () =>
        {
            CUtils.ShowInterstitialAd();
        });
        GameManager.PlayButton();
    }

    public void HideMenu()
    {
        menuPopup.HidePopup();
    }

    public void ShowExit()
    {
        exitPopup.ShowPopup();
        Timer.Schedule(this, 0.25f, () =>
        {
            CUtils.ShowInterstitialAd();
        });
        GameManager.PlayButton();
    }

    public void HideExit()
    {
        exitPopup.HidePopup();
    }

    public void CloseGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
    }
}
