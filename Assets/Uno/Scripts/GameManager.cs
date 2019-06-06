using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const int TOTAL_AVATAR = 15;
    public static AudioSource audioSource;
    public static GameMode currentGameMode = GameMode.Computer;
    public AudioClip buttonClip;
    public static AudioClip _buttonClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _buttonClip = buttonClip;
    }

    public static void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("sound File not set");
        }
        if (IsSound)
            audioSource.PlayOneShot(clip);

    }

    public static void PlayButton()
    {
        if (IsSound)
            audioSource.PlayOneShot(_buttonClip);
    }

    public void _PlayButton()
    {
        PlayButton();
    }

    public static bool IsSound
    {
        get
        {
            return PlayerPrefs.GetInt("isSound", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("isSound", value ? 1 : 0);
        }
    }

    public static bool IsFirstOpen
    {
        get
        {
            return PlayerPrefs.GetInt("isFirstOpen", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("isFirstOpen", value ? 1 : 0);
        }
    }

    public static string PlayerAvatarName
    {
        get
        {
            return PlayerPrefs.GetString("PlayerName", "");
        }
        set
        {
            PlayerPrefs.SetString("PlayerName", value);
        }
    }

    public static int PlayerAvatarIndex
    {
        get
        {
            return PlayerPrefs.GetInt("PlayerAvatarIndex", 0);
        }
        set
        {
            PlayerPrefs.SetInt("PlayerAvatarIndex", value);
        }
    }

    public static AvatarProfile PlayerAvatarProfile
    {
        get
        {
            return new AvatarProfile { avatarIndex = PlayerAvatarIndex, avatarName = PlayerAvatarName };
        }
    }
}

public static class ExtantionMethods
{
    public static Color GetColor(this CardType c)
    {
        switch (c)
        {
            case CardType.Red:
                return new Color(252f / 255f, 41f / 255f, 1f / 255f, 1f);
            case CardType.Yellow:
                return new Color(253f / 255f, 179f / 255f, 1f / 255f, 1f);
            case CardType.Green:
                return new Color(0f / 255f, 200f / 255f, 0f / 255f, 1f);
            case CardType.Blue:
                return new Color(0f / 255f, 120f / 255f, 253f / 255f, 1f);
            default:
                return new Color(1f, 1f, 1f, 1f);
        }

    }

    public static Color GetGrayColor(this CardType c)
    {
        switch (c)
        {
            case CardType.Red:
                return new Color(125f / 255f, 20f / 255f, 0f, 1f);
            case CardType.Yellow:
                return new Color(127f / 255f, 90f / 255f, 0f, 1f);
            case CardType.Green:
                return new Color(0, 100f / 255f, 0f, 1f);
            case CardType.Blue:
                return new Color(0f, 60f / 255f, 126f / 255f, 1f);
            default:
                return new Color(1f, 1f, 1f, 1f);
        }

    }

    public static void Shuffle<E>(this List<E> inputList)
    {
        for (int i = 0; i < inputList.Count; i++)
        {
            int index = Random.Range(0, inputList.Count);
            E temp = inputList[i];
            inputList[i] = inputList[index];
            inputList[index] = temp;
        }
    }
}
public enum GameMode
{
    Computer,
    MultiPlayer
}
public enum CardType
{
    Other,
    Red,
    Yellow,
    Green,
    Blue
}
public enum CardValue
{
    Zero,
    One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
    Skip, Reverse, DrawTwo,
    Wild, DrawFour
}