using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CardVisuals : MonoBehaviour
{
    public Card Card { get; set; }

    public Canvas Canvas => canvas;
    public CanvasGroup CanvasGroup => canvasGroup;

    public Image Background => background;
    public Image CostBackground => costBackground;
    public Image CardImage => cardImage;
    public TMP_Text CostText => costText;
    public Image Checkmark => checkmark;
    public TMP_Text TitleText => titleText;
    public TMP_Text TagsText => tagsText;
    public TMP_Text DescriptionText => descriptionText;

    //Card components
    [CacheComponent]
    [SerializeField] private CardObject cardObject;
    [CacheComponent]
    [SerializeField] private Canvas canvas;
    [CacheComponent]
    [SerializeField] private CanvasGroup canvasGroup;

    [Space]
    [SerializeField] private Image background;
    [SerializeField] private Image costBackground;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image checkmark;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tagsText;
    [SerializeField] private TMP_Text descriptionText;

    public void UpdateCardLook()
    {
        //�ndra background sprite till r�tt background depending on tier
        //�ndra costBackground till r�tt f�rg depending on cost type
        costBackground.color = Color.blue;
        cardImage.sprite = Card.Sprite;

        titleText.text = Card.DisplayName.ToString();
        UpdateCostLook();
        UpdateTagText();
        descriptionText.text = Card.Description;
    }

    public void UpdateCostLook()
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

    public void CheckmarkAppear()
    {
        checkmark.DOKill();
        checkmark.DOFade(1, 0.1f);
    }

    public void CheckmarkDisappear()
    {
        checkmark.DOKill();
        checkmark.DOFade(0, 0.1f);
    }
}
