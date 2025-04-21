using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    //Card components
    [SerializeField] private Image background;
    [SerializeField] private Image costBackground;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tagsText;
    [SerializeField] private TMP_Text descriptionText;




    [SerializeField] private Card card;
    [SerializeField] private Target user;
    [SerializeField] private Card[] cards;

    private bool _setCard;

    private void Start()
    {
        if (!_setCard)
        {
            BecomeRandomCard();
        }
        //gameObject.GetComponent<SpriteRenderer>().sprite = card.Sprite;
        UpdateCardLook();
    }
    private void UpdateCardLook()
    {
        //ändra background sprite till rätt background depending on tier
        //ändra costBackground till rätt färg depending on cost type
        costBackground.color = Color.blue;
        cardImage.sprite = card.Sprite;
        costText.text = card.Cost.ToString();
        titleText.text = card.DisplayName.ToString();
        UpdateTagText();
        descriptionText.text = card.Description; //.Description returnar inget rn
    }

    private void UpdateTagText()
    {
        if (card.Tags == null || card.Tags.Length <= 0)
            return;
        string tags = "";
        foreach (CardTag tag in card.Tags)
        {
            if (tag == null)
                continue;
            tags += tag.name;
            tags += " ";
        }
        tagsText.text = tags;
    }

    //Method to update mana cost text when mana affecting cards have been played
    public void UpdateCostLook()
    {
        costText.text = card.Cost.ToString();
    }

    private void BecomeRandomCard()
    {
        int i = Random.Range(0, cards.Length);
        SetCard(cards[i]);
    }

    public void SetCard(Card card)
    {
        this.card = card;
        _setCard = true;
    }

    public int GetCost()
    {
        return card.Cost;
    }

    public void Play()
    {
        card.Play(user, OnFinishPlayingCard);
    }

    private void OnFinishPlayingCard()
    {
        Debug.Log("I'm finish");
        //remove mana from target??

        //destroy itself
        StartCoroutine(DeleteItself());
    }

    IEnumerator DeleteItself()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

}
