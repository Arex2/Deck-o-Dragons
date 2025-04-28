using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Net;

public class CardVisuals : MonoBehaviour
{
    public Card Card { get; set; }

    public Canvas Canvas => canvas;
    public CanvasGroup CanvasGroup => canvasGroup;
    public UIDissolve Dissolve => dissolve;

    public Image Background => background;
    public Image CostBackground => costBackground;
    public Image CardImage => cardImage;

    public TMP_Text CostText => costText;
    public TMP_Text TitleText => titleText;
    public TMP_Text TagsText => tagsText;
    public TMP_Text DescriptionText => descriptionText;

    public Image Checkmark => checkmark;
    public TMP_Text DisabledText => disabledText;

    //Card components
    [CacheComponent]
    [SerializeField] private CardObject cardObject;
    [CacheComponent]
    [SerializeField] private Canvas canvas;
    [CacheComponent]
    [SerializeField] private CanvasGroup canvasGroup;
    [CacheComponent]
    [SerializeField] private UIDissolve dissolve;
    [SerializeField] private UIDissolve dissolveForEffects;

    [Space]
    [SerializeField] private Image background;
    [SerializeField] private Image costBackground;
    [SerializeField] private Image cardImage;

    [Space]
    [SerializeField] private Graphic overlayGraphic;

    [Header("Main Text Objects")]
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tagsText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Selecting Cards")]
    [SerializeField] private Image checkmark;
    [SerializeField] private TMP_Text disabledText;

    private void LateUpdate()
    {
        if (dissolveForEffects == null || dissolve == null)
        {
            return;
        }

        dissolveForEffects.DissolveAmount = dissolve.DissolveAmount;
    }

    public void RandomizeDissolve()
    {
        if (dissolveForEffects == null || dissolve == null)
        {
            return;
        }
        
        dissolve.Rotation = Random.Range(0, 360f);

        dissolveForEffects.RotationRadians = dissolve.RotationRadians;
        dissolveForEffects.Scale = dissolve.Scale;
    }

    public void UpdateCardLook()
    {
        //�ndra background sprite till r�tt background depending on level
        //�ndra costBackground till r�tt f�rg depending on cost type
        //costBackground.color = Color.blue;
        cardImage.sprite = Card.Sprite;

        titleText.text = Card.DisplayName.ToString();

        if (cardObject != null && cardObject.Level != 0)
        {
            if (cardObject.Level > 0)
            {
                titleText.text += $" (+{cardObject.Level})";
            }
            else
            {
                titleText.text += $" ({cardObject.Level})";
            }
        }
        UpdateCostWhole();
        UpdateTagText();

        if (cardObject == null)
        {
            descriptionText.text = Card.GetDescription();
        }
        else
        {
            descriptionText.text = cardObject.GetDescription();
        }
    }

    //makes card look like you CAN NOT play it
    public void UpdateCardUnavailableLook()
    {
        costBackground.color = Color.gray;
    }
    //makes card look like you CAN play it
    public void UpdateCardAvailableLook()
    {
        UpdateCostWhole();
    }

    private void UpdateCostWhole()
    {
        if (Card.Cost > 0 || Card.GetCardComponents<CardAttack>() == null)
        {
            //MANA
            costBackground.color = Color.blue;
            UpdateCostText();
            return;
        }

        float totalSelfDamage = 0;

        foreach (CardAttack attack in Card.GetCardComponents<CardAttack>())
        {
            if (attack.TargetFilter.Team == TargetFilter.FilterTeam.Own)
            {
                totalSelfDamage += attack.TotalDamage;
            }
        }

        if(totalSelfDamage > 0)
        {
            //HP
            Debug.LogWarning("THis card uses hp to play");
            costBackground.color = Color.red;
            costText.text = totalSelfDamage.ToString();
        }
        else
        {
            //MANA
            Debug.Log("THis card uses MANA to play");
            costBackground.color = Color.blue;
            UpdateCostText();
        }
    }

    public void UpdateCostText()
    {
        costText.text = (cardObject == null ? Card.Cost : cardObject.GetCost()).ToString();
    }

    public void UpdateTagText()
    {
        tagsText.text = GetTagsString(Card);
    }

    public static string GetTagsString(Card card)
    {
        if (card.Tags == null || card.Tags.Length <= 0)
        {
            return "";
        }

        string tags = "";
        foreach (CardTag tag in card.Tags)
        {
            if (tag == null || tag.Hidden)
                continue;

            tags += tag.DisplayName;
            tags += " ";
        }

        return tags;
    }

    public void FadeOverlay(float alpha, float duration)
    {
        overlayGraphic.DOKill();
        overlayGraphic.DOFade(alpha, duration);
    }

    public void SetOverlayColor(Color color)
    {
        overlayGraphic.DOKill();

        Color temp = color;
        temp.a = overlayGraphic.color.a;
        overlayGraphic.color = temp;
    }

    public void ToggleCheckmark(bool visible)
    {
        if (visible)
        {
            checkmark.DOKill();
            checkmark.DOFade(1, 0.1f);
        }
        else
        {
            checkmark.DOKill();
            checkmark.DOFade(0, 0.1f);
        }
    }

    public void SetDisabledText(string text) => disabledText.text = text;

    public void ToggleDisabledText(bool visible)
    {
        if (visible)
        {
            disabledText.DOKill();
            disabledText.DOFade(1, 0.1f);
        }
        else
        {
            disabledText.DOKill();
            disabledText.DOFade(0, 0.1f);
        }
    }
}
