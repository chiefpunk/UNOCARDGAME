using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class Card : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] bool _isOpen = true;
    [SerializeField] bool _isClickable;
    [SerializeField] CardType _type;
    [SerializeField] CardValue _value;

    [Space(20)] [SerializeField] Text label1;
    [SerializeField] Text label2;
    [SerializeField] Text label3;
    [SerializeField] float moveSpeed = 0.3f;

    [HideInInspector]
    public int point;

    public CardType Type
    {
        get
        { return _type; }
        set
        {
            _type = value;
        }
    }
    public CardValue Value
    {
        get
        { return _value; }
        set
        {
            _value = value;
        }
    }
    public bool IsOpen
    {
        get
        { return _isOpen; }
        set
        {
            _isOpen = value;
            UpdateCard();
        }
    }

    public bool IsClickable
    {
        get
        {
            return _isClickable;
        }
        set
        {
            _isClickable = value;
        }
    }
    public Action<Card> onClick;

    public void SetTargetPosAndRot(Vector3 pos, float rotZ)
    {
        if (LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject);
        float t = Vector2.Distance(transform.localPosition, pos) * moveSpeed / 1000f;
        LeanTween.moveLocal(gameObject, pos, t);
        LeanTween.rotateLocal(gameObject, new Vector3(0f, 0f, rotZ), t);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            UpdateCard();
#endif
    }

    public void UpdateCard()
    {
        string txt = "";
        string sprite = "Cards/BlankCard";
        if (IsOpen)
        {
            if (Type == CardType.Other)
            {
                if (Value == CardValue.DrawFour)
                    sprite = "Cards/DrawFour";
                else if (Value == CardValue.Wild)
                    sprite = "Cards/Wild";
            }
            else
            {
                int value = (int)Value;
                if (value <= 9)
                {
                    sprite = "Cards/Number_" + (int)Type;
                    if (value == 6 || value == 9) sprite += "_Underline";
                    txt = value + "";
                }
                else
                {
                    sprite = "Cards/" + Value.ToString() + "_" + (int)Type;
                    if (Value == CardValue.DrawTwo)
                        txt = "+2";
                }
            }
        }

        GetComponent<Image>().sprite = Resources.Load<Sprite>(sprite);
        label2.color = Type.GetColor();

        label1.text = txt;
        label2.text = (Value == CardValue.DrawTwo) ? "" : txt;
        label3.text = txt;
    }

    public void CalcPoint()
    {
        if (Type == CardType.Other)
        {
            point = 40;
        }
        else if (Value == CardValue.Reverse || Value == CardValue.DrawTwo || Value == CardValue.Skip)
        {
            point = 20;
        }
        else
        {
            point = (int)Value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsOpen && IsClickable && onClick != null)
        {
            onClick.Invoke(this);
        }
    }
    public bool IsAllowCard()
    {
        return Type == GamePlayManager.instance.CurrentType ||
            Value == GamePlayManager.instance.CurrentValue ||
            Type == CardType.Other;
    }

    public void SetGaryColor(bool b)
    {
        if (b && IsOpen)
        {
            GetComponent<Image>().color = Color.gray;
            label1.color = Color.gray;
            label2.color = Type.GetGrayColor();
            label3.color = Color.gray;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
            label1.color = Color.white;
            label2.color = Type.GetColor();
            label3.color = Color.white;
        }
    }
}
