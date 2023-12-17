namespace Game.Gates
{
    public class GateData
    {
        public GateType Type;
        public int Amount;

        public GateData(GateType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }

    public enum GateType
    {
        StickMan,
        Year,
        Power,
        Rate,
        Range
    }
}