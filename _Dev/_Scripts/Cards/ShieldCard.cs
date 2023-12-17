namespace Game.Cards
{
    public class ShieldCard : AddCard
    {
        #region PROTECTED METHODS

        protected override void Init()
        {
            SetAmountText();

            backgroundSprite.sprite = cardData.BackgroundSprites[cardData.BackgroundIndex - 1];
            frontSprite.sprite = cardData.FrontSprites[0];
        }

        protected override void SetAmountText()
        {
            amountText.text = $"Shield Lv.{amount}";
        }

        #endregion
    }
}