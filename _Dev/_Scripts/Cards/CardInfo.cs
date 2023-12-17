namespace Game.Cards
{
    public class CardInfo
    {
        public CardType CardType;
        public ModifierType ModifierType;
        public int Amount;

        public CardInfo(CardType cardType, ModifierType modifierType, int amount)
        {
            CardType = cardType;
            ModifierType = modifierType;
            Amount = amount;
        }
    }

    public enum CardType
    {
        StickMan,
        Rate,
        Power,
        Range,
        Year,
        DoubleShot,
        TripleShot,
        RicochetProjectile,
        BoomerangProjectile,
        IceShot,
        FireShot,
        Shield,
        MoneyMultiplier
    }

    public enum ModifierType
    {
        Add,
        Multiply,
        Unlock
    }
}