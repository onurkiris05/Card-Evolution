using TMPro;
using UnityEngine;

namespace Game.Cards
{
    public class AddCard : CardBase
    {
        [SerializeField] protected TextMeshProUGUI headerText;


        #region UNITY EVENTS

        protected override void Awake()
        {
            base.Awake();

            Init();
        }

        #endregion

        #region PUBLIC METHODS

        public override CardInfo GetCardData()
        {
            var data = new CardInfo
            (
                cardType,
                ModifierType.Add,
                amount
            );

            return data;
        }

        #endregion

        #region PROTECTED METHODS

        protected override void Init()
        {
            SetAmountText();

            backgroundSprite.sprite = cardData.BackgroundSprites[cardData.BackgroundIndex - 1];

            switch (cardType)
            {
                case CardType.StickMan:
                    frontSprite.sprite = cardData.FrontSprites[0];
                    break;
                case CardType.Rate:
                    frontSprite.sprite = cardData.FrontSprites[1];
                    headerText.text = "Fire Rate";
                    headerText.gameObject.SetActive(true);
                    break;
                case CardType.Power:
                    frontSprite.sprite = cardData.FrontSprites[2];
                    headerText.text = "Fire Power";
                    headerText.gameObject.SetActive(true);
                    break;
                case CardType.Range:
                    frontSprite.sprite = cardData.FrontSprites[3];
                    headerText.text = "Fire Range";
                    headerText.transform.localPosition = new Vector3(headerText.transform.localPosition.x, 65, headerText.transform.localPosition.z);
                    headerText.gameObject.SetActive(true);
                    break;
                case CardType.Year:
                    frontSprite.sprite = cardData.FrontSprites[4];
                    break;
                case CardType.Shield:
                    frontSprite.sprite = cardData.FrontSprites[5];
                    break;
            }
        }

        protected override void SetAmountText()
        {
            amountText.text = $"+{amount}";
        }

        #endregion
    }
}